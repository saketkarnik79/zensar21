# Clean Architecture vs CQRS for High-Traffic E-Commerce Systems

## Executive Summary

**Clean Architecture** provides a well-organized, layered approach suitable for most business applications with moderate to high complexity.

**CQRS (Command Query Responsibility Segregation)** optimizes high-traffic systems by separating read and write operations, enabling independent scaling and optimization.

For the Order Management System, this document compares both approaches across **scalability**, **complexity**, and **maintenance**.

---

## 1. ARCHITECTURE OVERVIEW

### 1.1 Clean Architecture (Current OMS Design)

**Structure:**

```
┌────────────────────────────────────────┐
│        Presentation Layer (API)        │
│  Controllers, DTOs, Middleware         │
└──────────────┬─────────────────────────┘
               │ Depends On
┌──────────────▼─────────────────────────┐
│      Application Layer                 │
│  Use Cases, Services, Validators       │
└──────────────┬─────────────────────────┘
               │ Depends On
┌──────────────▼─────────────────────────┐
│       Domain Layer                     │
│  Entities, Value Objects, Events       │
└──────────────┬─────────────────────────┘
               │ Depends On
┌──────────────▼─────────────────────────┐
│     Infrastructure Layer               │
│  Repositories, DbContext, EF Core      │
└────────────────────────────────────────┘
```

**Single Database Model:**

- One canonical model for reads and writes
- Domain entities represent both command and query data
- Repository abstracts all data access

**Data Flow:**

```
Request → Controller → UseCase → Repository → Database
Response ← Controller ← UseCase ← Repository ← Database
```

**Example: Get Order with Clean Architecture**

```csharp
[HttpGet("{orderId}")]
public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken ct)
{
    var order = await _orderRepository.GetByIdAsync(
        OrderId.Create(orderId), ct);

    return Ok(_mapper.Map<OrderResponseDto>(order));
}

// Repository reads the same Order entity used for writes
public async Task<Order> GetByIdAsync(OrderId id, CancellationToken ct)
{
    return await _context.Orders
        .Include(o => o.Items)
        .Include(o => o.Payment)
        .FirstOrDefaultAsync(o => o.Id == id, ct);
}
```

---

### 1.2 CQRS Architecture

**Structure:**

```
                    ┌─────────────────────────┐
                    │   Client Application    │
                    └────────┬────────────────┘
                             │
          ┌──────────────────┴──────────────────┐
          │                                     │
    ┌─────▼──────┐                      ┌─────▼──────┐
    │  Commands  │                      │   Queries  │
    │ (Write)    │                      │  (Read)    │
    └─────┬──────┘                      └─────┬──────┘
          │                                   │
    ┌─────▼──────────────────┐         ┌─────▼──────────────────┐
    │ Command Handler Layer  │         │ Query Handler Layer    │
    └─────┬──────────────────┘         └─────┬──────────────────┘
          │                                   │
    ┌─────▼──────────────────┐         ┌─────▼──────────────────┐
    │   Write Database       │         │   Read Database        │
    │  (Normalized)          │         │  (Denormalized)        │
    │  ┌─────────────────┐   │         │  ┌─────────────────┐   │
    │  │ Orders          │   │         │  │ OrderReads      │   │
    │  │ OrderItems      │   │         │  │ (Materialized)  │   │
    │  │ Inventory       │   │         │  │ OrderSummary    │   │
    │  │ Payments        │   │         │  │ OrderTimeline   │   │
    │  └─────────────────┘   │         │  └─────────────────┘   │
    └────────────────────────┘         └────────────────────────┘
          │                                   │
          └───────────┬──────────────────────┘
                      │
             ┌────────▼─────────┐
             │  Event Bus       │
             │ (Integration)    │
             │ ┌──────────────┐ │
             │ │Domain Events │ │
             │ │Update Read   │ │
             │ │Database      │ │
             │ └──────────────┘ │
             └──────────────────┘
```

**Separate Read and Write Models:**

- Commands modify the write database (normalized)
- Queries read from read database (denormalized)
- Events synchronize read from write
- Independent scaling of read and write paths

**Data Flow:**

```
CREATE ORDER REQUEST
  ↓
CreateOrderCommand
  ↓
CommandHandler
  ├→ Validate
  ├→ Update Write DB (normalized)
  └→ Publish OrderCreatedEvent
       ↓
     Event Handler
       ├→ Update OrderSummary in Read DB
       ├→ Update OrderTimeline in Read DB
       └→ Update Cache

GET ORDER REQUEST
  ↓
GetOrderQuery
  ↓
QueryHandler
  └→ Read from Materialized View (Read DB)
     ↓
  Return Denormalized Data (already optimized)
```

**Example: Get Order with CQRS**

```csharp
[HttpGet("{orderId}")]
public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken ct)
{
    var query = new GetOrderQuery(orderId);
    var result = await _queryHandler.HandleAsync(query, ct);
    return Ok(result);
}

// Query reads from pre-optimized, denormalized view
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderReadModel>
{
    private readonly IQueryRepository _queryRepository;

    public async Task<OrderReadModel> HandleAsync(
        GetOrderQuery query,
        CancellationToken ct)
    {
        // Reads from OrderSummary materialized view
        // Already includes all necessary data (no N+1 problem)
        // Optimized for read performance
        return await _queryRepository.GetOrderSummaryAsync(query.OrderId, ct);
    }
}

// OrderSummary is a denormalized read model
public class OrderReadModel
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public List<OrderItemReadModel> Items { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
```

---

## 2. SCALABILITY COMPARISON

### 2.1 Clean Architecture Scalability

**Read/Write Ratio Problem:**

```
E-commerce Traffic Pattern:
├─ Read Operations: 80-90% (Browse products, view orders, search)
├─ Write Operations: 10-20% (Create order, update inventory)
└─ Single Database Bottleneck: Reads contend with writes
```

**Horizontal Scaling:**

```
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│ API Instance 1   │     │ API Instance 2   │     │ API Instance 3   │
│ ├─ Read 80 req/s │     │ ├─ Read 80 req/s │     │ ├─ Read 80 req/s │
│ └─ Write 20 req/s│     │ └─ Write 20 req/s│     │ └─ Write 20 req/s│
└────────┬─────────┘     └────────┬─────────┘     └────────┬─────────┘
         │                        │                        │
         └────────────┬───────────┴────────────┬───────────┘
                      │
              ┌───────▼────────┐
              │  Shared Database│
              │ (All 300 req/s) │
              │ Bottleneck!     │
              └─────────────────┘
```

**Database Optimization Options:**

1. **Connection Pooling** - Limits: 100-200 concurrent connections per server
2. **Read Replicas** - Partial solution; eventual consistency issues
3. **Caching** - Helps but still requires write coordination
4. **Vertical Scaling** - Expensive; limited by hardware

**Scaling Challenges:**

- All writes go to primary database
- Complex joins for read-heavy queries require optimization
- Cache invalidation becomes complex
- N+1 query problems under load

---

### 2.2 CQRS Scalability

**Independent Scaling:**

```
Read Path (80-90% of traffic)          Write Path (10-20% of traffic)
┌──────────────────────┐              ┌──────────────────────┐
│ Read API Instances   │              │ Write API Instances  │
│ (Horizontal scale)   │              │ (Limited scaling)    │
│ ┌────────────────┐   │              │ ┌────────────────┐   │
│ │ Instance 1     │   │              │ │ Instance 1     │   │
│ │ Instance 2     │   │              │ │ Instance 2     │   │
│ │ Instance 3     │   │              │ └────────────────┘   │
│ │ Instance 4 ←──────────────────────► Instance 3 ←────────┐
│ │ Instance 5     │   │              │ (Limited)           │
│ │ (+ More as     │   │              │                     │
│ │  needed)       │   │              │ Domain Event Bus    │
│ └────────────────┘   │              │ (Event Streaming)   │
│         ↓            │              └─────────┬──────────┘
│  ┌───────────────┐   │                        │
│  │ Read Database │   │                        │
│  │ (Optimized    │   │              ┌─────────▼──────────┐
│  │  for queries) │   │              │ Write Database     │
│  │ ┌───────────┐ │   │              │ (Optimized for     │
│  │ │ OrderView │ │   │              │  transactions)     │
│  │ │ Summary   │ │   │              │ ┌────────────────┐ │
│  │ │ Cache     │ │   │              │ │ Order Aggregate│ │
│  │ └───────────┘ │   │              │ │ Inventory      │ │
│  └───────────────┘   │              │ │ Payment        │ │
└──────────────────────┘              │ └────────────────┘ │
                                      └────────────────────┘
```

**Scaling Benefits:**

1. **Read Optimization** - Add read replicas, caching layers, distributed cache
2. **Write Optimization** - Focus on transaction consistency, not query performance
3. **Independent Tuning** - Read DB can use different schema/indexes than write DB
4. **Event-Driven Updates** - Asynchronous read model refresh prevents blocking
5. **Horizontal Scaling** - Read instances scale independently; writes stay consistent

**Example Scaling Configuration:**

```csharp
// Read Side - Optimized for queries
services.AddStackExchangeRedisCache(options =>
    options.Configuration = "redis-cache-cluster:6379");

services.AddScoped<IQueryRepository, OptimizedReadRepository>();

// Can add multiple read replicas
services.AddScoped<IReadModelRepository, ElasticsearchRepository>();

// Write Side - Optimized for consistency
services.AddDbContext<WriteDbContext>(options =>
    options.UseSqlServer(writeConnectionString)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));

services.AddScoped<IEventStore, SqlServerEventStore>();

// Event-driven updates
services.AddSingleton(sp =>
    new DomainEventBus(
        sqlServerPublisher,
        new OrderCreatedEventHandler(readModelUpdater)));
```

**Performance Under Load:**

```
CQRS:
├─ Read Performance: O(1) - Direct table lookups, pre-aggregated
├─ Write Consistency: Strong ACID on write DB
├─ Read Freshness: Eventually consistent (milliseconds to seconds)
└─ Scalability: Linear (add instances independently)

Clean Architecture:
├─ Read Performance: O(n) joins if not cached
├─ Write Consistency: Strong ACID
├─ Read Freshness: Always current (but slower)
└─ Scalability: Bottleneck at database
```

---

## 3. COMPLEXITY COMPARISON

### 3.1 Clean Architecture Complexity

**Advantages:**

- **Single Model** - One entity represents truth; easier to reason about
- **Simpler Transaction Handling** - All changes in one transaction
- **Straightforward Debugging** - Data inconsistencies easier to trace
- **Lower Learning Curve** - Familiar layered pattern

**Code Complexity:**

```csharp
// Clean Architecture: Single model used for reads and writes
public class OrderRepository : IOrderRepository
{
    public async Task<Order> GetByIdAsync(OrderId id, CancellationToken ct)
    {
        // Reads the domain entity
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct)
    {
        // Same entity type
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(ct);
    }
}

// Complexity is moderate: one entity for all scenarios
public Order MapToEntity(CreateOrderDto dto)
{
    return Order.Create(dto.CustomerId, dto.Items, dto.Address);
}
```

**Complexity Challenges:**

- **Rich Domain Models** - Entities grow to support all use cases
- **Eager Loading Issues** - N+1 queries without careful optimization
- **Cache Invalidation** - Maintaining cache coherence is complex
- **Performance Tuning** - Increasingly difficult at scale

---

### 3.2 CQRS Complexity

**Disadvantages:**

- **Multiple Models** - Separate read and write representations
- **Event Consistency** - Read DB eventually consistent
- **Distributed Coordination** - Events, subscribers, failures
- **Higher Learning Curve** - Requires event-driven thinking

**Code Complexity:**

```csharp
// CQRS: Separate models for commands and queries

// Write Model (Domain Entity - normalized)
public class Order : AggregateRoot
{
    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public OrderStatus Status { get; private set; }

    public void ProcessPayment(PaymentResult result)
    {
        // Business logic only
        Status = result.IsSuccessful
            ? OrderStatus.Processing
            : OrderStatus.Cancelled;

        // Raise event for read model update
        RaiseDomainEvent(new OrderPaymentProcessedEvent(
            Id, CustomerId, Status, TotalAmount));
    }
}

// Read Model (Denormalized for queries)
public class OrderReadModel
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public List<OrderItemReadModel> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    // Pre-calculated fields
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

// Command Handler (Write)
public class ProcessOrderPaymentCommandHandler
{
    public async Task HandleAsync(
        ProcessOrderPaymentCommand command,
        CancellationToken ct)
    {
        var order = await _writeRepository.GetByIdAsync(
            command.OrderId, ct);

        order.ProcessPayment(paymentResult);

        await _writeRepository.UpdateAsync(order, ct);

        // Events are published, but not awaited
        // Read model updates happen asynchronously
    }
}

// Query Handler (Read)
public class GetOrderQueryHandler
{
    public async Task<OrderReadModel> HandleAsync(
        GetOrderQuery query,
        CancellationToken ct)
    {
        // Direct query on optimized read model
        return await _readRepository.GetAsync(query.OrderId, ct);
    }
}

// Event Handler (Synchronizes read model)
public class OrderPaymentProcessedEventHandler
{
    public async Task HandleAsync(
        OrderPaymentProcessedEvent evt,
        CancellationToken ct)
    {
        // Update read model asynchronously
        var readModel = await _readRepository.GetAsync(evt.OrderId, ct);
        readModel.Status = evt.NewStatus.Value;
        await _readRepository.UpdateAsync(readModel, ct);
    }
}
```

**Complexity Advantages:**

- **Separation of Concerns** - Read and write logic independent
- **Flexible Read Models** - Different denormalizations for different queries
- **Event Sourcing Compatible** - Can audit all state changes
- **Testing Isolation** - Command and query paths tested separately

**Complexity Management Strategies:**

1. **Use a CQRS Framework** - MediatR, Sleet, or custom
2. **Event Bus Abstraction** - Hide distribution complexity
3. **Event Handler Conventions** - Automatic registration
4. **Read Model Generation** - Script-based denormalization

---

## 4. MAINTENANCE COMPARISON

### 4.1 Clean Architecture Maintenance

**Maintenance Advantages:**

```
✓ Single Schema - One database schema to manage
✓ Entity Consistency - Changes propagate naturally
✓ Simpler Migrations - One migration per feature
✓ Debugging - Direct relationship between code and data
```

**Example: Adding a Field**

```csharp
// 1. Add to domain entity
public class Order : AggregateRoot
{
    public string TrackingNumber { get; set; }  // NEW
}

// 2. Add to EF Core configuration
modelBuilder.Entity<Order>()
    .Property(o => o.TrackingNumber)
    .HasMaxLength(50);

// 3. Create migration
// dotnet ef migrations add AddTrackingNumberToOrder

// 4. Update repository (if needed)
// That's it! Already loaded by Include()

// 5. Update DTO
public class OrderResponseDto
{
    public string TrackingNumber { get; set; }  // NEW
}
```

**Maintenance Challenges:**

- **Changing Requirements** - Need to update entire stack
- **Performance Regression** - No isolation between read/write concerns
- **Scaling Awkwardness** - Hard to add read-specific optimizations
- **Refactoring Risk** - Changes affect all use cases

---

### 4.2 CQRS Maintenance

**Maintenance Advantages:**

```
✓ Isolated Changes - Read/write path changes independent
✓ Read Model Flexibility - Add views without affecting domain
✓ Event Log - Complete audit trail of changes
✓ Experimentation - Add new read models safely
```

**Example: Adding a Field to Read Model Only**

```csharp
// 1. Add to read model (no write model change)
public class OrderReadModel
{
    public string TrackingNumber { get; set; }  // NEW - read only
}

// 2. Create read database migration
// Migration: Add TrackingNumber column to OrderReadView

// 3. Update event handler
public class OrderShippedEventHandler
{
    public async Task HandleAsync(
        OrderShippedEvent evt,
        CancellationToken ct)
    {
        var readModel = await _readRepository.GetAsync(evt.OrderId, ct);
        readModel.TrackingNumber = evt.TrackingNumber;  // NEW
        await _readRepository.UpdateAsync(readModel, ct);
    }
}

// No changes needed to write model or commands!
```

**Maintenance Challenges:**

- **Schema Proliferation** - Multiple databases to manage
- **Consistency Management** - Event synchronization failures
- **Debugging Complexity** - Eventual consistency requires tracing events
- **Deployment Coordination** - Write and read model migrations must be compatible

**Event Handler Failure Pattern:**

```
┌─ Order Created Event ─────────┐
│                               │
├─→ OrderCreatedEventHandler    │ ✓ Success
│   Updates OrderSummary View   │
│                               │
├─→ AnalyticsEventHandler       │ ✗ Fails (timeout)
│   Updates Dashboard Cache     │ (Network error)
│                               │
└─ Result: Inconsistency       │
   Dashboard outdated but      │
   OrderSummary updated        │
```

**Mitigation:**

- Dead letter queues for failed events
- Event replay mechanism
- Idempotent handlers (critical!)
- Monitoring and alerting

---

## 5. TRADE-OFF MATRIX

| Aspect                     | Clean Architecture        | CQRS                       |
| -------------------------- | ------------------------- | -------------------------- |
| **Scalability**            | ⭐⭐⭐ Limited            | ⭐⭐⭐⭐⭐ Excellent       |
| **Read Performance**       | ⭐⭐⭐ Depends on caching | ⭐⭐⭐⭐⭐ Optimized views |
| **Write Performance**      | ⭐⭐⭐⭐ Direct           | ⭐⭐⭐⭐ Direct            |
| **Code Complexity**        | ⭐⭐⭐⭐ Simple           | ⭐⭐ Complex               |
| **Operational Complexity** | ⭐⭐⭐⭐ Simple           | ⭐⭐ Complex               |
| **Data Consistency**       | ⭐⭐⭐⭐⭐ Strong         | ⭐⭐⭐ Eventual            |
| **Maintenance**            | ⭐⭐⭐⭐ Easy             | ⭐⭐⭐ Medium              |
| **Learning Curve**         | ⭐⭐⭐⭐ Shallow          | ⭐⭐ Steep                 |
| **Testing**                | ⭐⭐⭐⭐ Straightforward  | ⭐⭐⭐ More scenarios      |
| **Flexibility**            | ⭐⭐⭐ Moderate           | ⭐⭐⭐⭐⭐ High            |

---

## 6. PRACTICAL RECOMMENDATIONS

### 6.1 Choose Clean Architecture When:

**Characteristics:**

- ✓ 50:50 or balanced read/write ratio
- ✓ Single database is sufficient
- ✓ < 1000 requests/second sustained
- ✓ Strong consistency requirement
- ✓ Small team with limited event-driven experience
- ✓ Quick time-to-market is critical
- ✓ Data model is relatively stable

**E-Commerce Examples:**

- Admin portals with moderate traffic
- B2B order management
- Inventory management systems
- Small to medium SaaS applications

**OMS Recommendation:** ✓ **Good fit** for baseline implementation

```
Current OMS Architecture: Clean Architecture
├─ Moderate complexity acceptable
├─ Strong consistency required for payments
├─ Single database manageable
└─ Good foundation for future evolution
```

---

### 6.2 Choose CQRS When:

**Characteristics:**

- ✓ 80:20 or higher read:write ratio
- ✓ > 1000 requests/second sustained
- ✓ Multiple independent read views needed
- ✓ Eventually consistent is acceptable
- ✓ Team comfortable with event-driven patterns
- ✓ Complex analytical or reporting queries
- ✓ Audit trail required

**E-Commerce Examples:**

- High-traffic product catalogs
- Real-time analytics dashboards
- Multi-tenant SaaS platforms
- Social media/collaboration tools

**OMS Upgrade Scenario:**

```
If OMS scales to high traffic:
├─ Maintain Clean Architecture for writes
├─ Add CQRS layer for reads:
│  ├─ OrderSummary materialized view
│  ├─ OrderTimeline denormalized
│  ├─ InventoryReads cache
│  └─ Analytics dashboard
├─ Keep event-based synchronization
└─ Gradual migration possible
```

---

### 6.3 Hybrid Approach (Recommended for High-Traffic E-Commerce)

**Strategy: Clean Architecture + CQRS Layer**

```
┌────────────────────────────────────────────────────────────┐
│               API Controllers (Express)                    │
├────────────────────────────────────────────────────────────┤
│ Write Commands                 │ Read Queries             │
│ ├─ CreateOrderCommand           │ ├─ GetOrderSummary      │
│ ├─ ProcessPaymentCommand        │ ├─ SearchOrders         │
│ └─ UpdateInventoryCommand       │ └─ GetDashboard         │
├────────────────────────────────────────────────────────────┤
│            Write Side (CA)           │  Read Side (CQRS)   │
│                                      │                     │
│ ┌─────────────────────────────┐    │ ┌──────────────────┐ │
│ │ CommandHandlers             │    │ │ QueryHandlers    │ │
│ │ ├─ CreateOrderHandler       │    │ │ ├─ GetOrderView  │ │
│ │ ├─ PaymentHandler           │    │ │ └─ GetAnalytics  │ │
│ │ └─ InventoryHandler         │    │ │                  │ │
│ │        ↓                     │    │ │        ↓         │ │
│ │ Write DB (Normalized)       │    │ │ Read DB (Denorm) │ │
│ │ ├─ Orders                   │    │ │ ├─ OrderViews    │ │
│ │ ├─ OrderItems               │    │ │ ├─ OrderTimeline │ │
│ │ ├─ Inventory                │    │ │ └─ Metrics       │ │
│ │ └─ Payments                 │    │ │                  │ │
│ └─────────────────────────────┘    │ └──────────────────┘ │
│        ↓ (Async Events)             │                     │
│ ┌─────────────────────────────┐    │                     │
│ │ Event Bus / Message Queue   │────┤ Event Handlers     │
│ │ (Redis/RabbitMQ/ServiceBus)│    │ Update Read DB      │
│ └─────────────────────────────┘    │                     │
└────────────────────────────────────────────────────────────┘
```

**Implementation Phases:**

**Phase 1: Clean Architecture (Foundation)**

```
✓ Implement complete OMS with CA
✓ All operations work efficiently
✓ Test at moderate load
✓ Establish baseline metrics
```

**Phase 2: Add Read Optimization (CQRS Layer)**

```
✓ Introduce read models
✓ Keep write side unchanged
✓ Migrate queries gradually
✓ Monitor consistency
```

**Phase 3: Scale Independent**

```
✓ Independent read instance scaling
✓ Implement distributed caching
✓ Add analytical views
✓ Optimize based on metrics
```

---

## 7. IMPLEMENTATION EXAMPLE: Hybrid CQRS for OMS

### 7.1 Clean Architecture Write Side (Unchanged)

```csharp
// Domain: Order Aggregate (Write Model)
public class Order : AggregateRoot
{
    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public List<OrderItem> Items { get; private set; }

    public void ProcessPayment(PaymentResult result)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Order not pending");

        Status = result.IsSuccessful
            ? OrderStatus.Processing
            : OrderStatus.Cancelled;

        // Emit event
        RaiseDomainEvent(new OrderPaymentProcessedEvent(
            Id, CustomerId, Status, TotalAmount));
    }
}

// Repository: Write only
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(OrderId id, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task UpdateAsync(Order order, CancellationToken ct);
}

// UseCase: Command Handler
public class ProcessPaymentUseCase
{
    public async Task<ProcessPaymentResponse> ExecuteAsync(
        ProcessPaymentRequest request,
        CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(
            OrderId.Create(request.OrderId), ct);

        var paymentResult = await _paymentGateway.ProcessAsync(...);

        order.ProcessPayment(paymentResult);

        await _orderRepository.UpdateAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Events published automatically by event dispatcher
        return new ProcessPaymentResponse(...);
    }
}
```

### 7.2 New CQRS Read Layer

```csharp
// Read Model (Denormalized)
public class OrderReadModel
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public List<OrderItemReadModel> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? TrackingNumber { get; set; }
}

// Query Handler
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderReadModel>
{
    private readonly IOrderReadRepository _readRepository;

    public async Task<OrderReadModel> HandleAsync(
        GetOrderQuery query,
        CancellationToken ct)
    {
        // Direct lookup - no joins needed
        return await _readRepository.GetOrderSummaryAsync(
            query.OrderId, ct);
    }
}

// Event Handler: Synchronizes read model
public class OrderPaymentProcessedEventHandler : IDomainEventHandler
{
    private readonly IOrderReadRepository _readRepository;

    public async Task HandleAsync(
        OrderPaymentProcessedEvent evt,
        CancellationToken ct)
    {
        var orderRead = await _readRepository.GetOrderSummaryAsync(
            evt.OrderId.Value, ct);

        orderRead.Status = evt.Status.Value;

        if (evt.Status == OrderStatus.Processing)
        {
            orderRead.CompletedAt = DateTime.UtcNow;
        }

        await _readRepository.UpdateAsync(orderRead, ct);
    }
}

// Event Handler: Update timeline
public class OrderCreatedEventHandler : IDomainEventHandler
{
    private readonly ITimelineRepository _timelineRepository;

    public async Task HandleAsync(
        OrderCreatedEvent evt,
        CancellationToken ct)
    {
        var timelineEntry = new OrderTimelineEntry
        {
            OrderId = evt.OrderId.Value,
            EventType = "OrderCreated",
            Message = $"Order created by customer {evt.CustomerId.Value}",
            Amount = evt.TotalAmount.Amount,
            CreatedAt = DateTime.UtcNow
        };

        await _timelineRepository.AddAsync(timelineEntry, ct);
    }
}
```

### 7.3 API: Single Endpoint, Different Paths

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ProcessPaymentUseCase _processPaymentUseCase;
    private readonly IQueryHandler<GetOrderQuery, OrderReadModel> _queryHandler;

    // WRITE: Clean Architecture path
    [HttpPost("process-payment")]
    public async Task<IActionResult> ProcessPayment(
        ProcessPaymentRequest request,
        CancellationToken ct)
    {
        var result = await _processPaymentUseCase.ExecuteAsync(request, ct);
        return Ok(result);
    }

    // READ: CQRS path
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(
        Guid orderId,
        CancellationToken ct)
    {
        var query = new GetOrderQuery(orderId);
        var result = await _queryHandler.HandleAsync(query, ct);
        return Ok(result);
    }
}
```

### 7.4 Dependency Injection Setup

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddHybridCQRS(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Clean Architecture (Write)
        services.AddScoped<ProcessPaymentUseCase>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("Write")));

        // CQRS (Read)
        services.AddScoped<IOrderReadRepository, OrderReadRepository>();
        services.AddDbContext<ReadDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("Read"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        // Query Handlers
        services.AddScoped(typeof(IQueryHandler<,>),
            typeof(GetOrderQueryHandler));

        // Event Handlers (Synchronization)
        services.AddScoped<IDomainEventHandler,
            OrderPaymentProcessedEventHandler>();
        services.AddScoped<IDomainEventHandler,
            OrderCreatedEventHandler>();

        // Event Bus
        services.AddScoped<IDomainEventPublisher,
            InMemoryDomainEventPublisher>();

        return services;
    }
}

// In Program.cs
builder.Services.AddHybridCQRS(builder.Configuration);
```

---

## 8. MIGRATION PATH: Clean → Hybrid → Full CQRS

### Timeline for Scaling

```
TRAFFIC PHASE 1 (100-500 req/s): Clean Architecture
├─ Implement baseline OMS
├─ Monitor metrics
├─ Identify bottlenecks
└─ Decision: Scale up or evolve?

TRAFFIC PHASE 2 (500-2000 req/s): Add CQRS Layer
├─ Keep write model (Clean Architecture)
├─ Add read models for high-traffic queries
├─ Implement event synchronization
├─ Test at load
└─ Result: Hybrid CQRS

TRAFFIC PHASE 3 (2000+ req/s): Full CQRS
├─ Event sourcing for write model
├─ Multiple read model projections
├─ Distributed event bus
├─ CQRS all the way
└─ Scale independently
```

---

## 9. DECISION FRAMEWORK

### Ask These Questions:

| Question                         | Answer → Recommendation                                |
| -------------------------------- | ------------------------------------------------------ |
| **What's peak traffic (req/s)?** | < 500 → CA / 500-2000 → Hybrid / > 2000 → CQRS         |
| **Read:Write ratio?**            | < 70:30 → CA / 70:30 - 90:10 → Hybrid / > 90:10 → CQRS |
| **Need multiple read views?**    | No → CA / Maybe → Hybrid / Yes → CQRS                  |
| **Eventually consistent OK?**    | No → CA / Maybe → Hybrid / Yes → CQRS                  |
| **Team experience?**             | Novice → CA / Intermediate → Hybrid / Expert → CQRS    |
| **Time to launch?**              | Weeks → CA / Months → Hybrid / Flexible → CQRS         |
| **Budget for infrastructure?**   | Limited → CA / Medium → Hybrid / Generous → CQRS       |

---

## 10. CONCLUSION

**For Your OMS:**

| Phase            | Architecture               | Rationale                                           |
| ---------------- | -------------------------- | --------------------------------------------------- |
| **MVP**          | Clean Architecture         | ✓ Simple, fast to market, proven pattern            |
| **1000+ req/s**  | Hybrid CQRS                | ✓ Scale reads independently, keep write consistency |
| **10000+ req/s** | Full CQRS + Event Sourcing | ✓ Maximum scalability, complete audit trail         |

**Start with Clean Architecture.** It's proven, maintainable, and provides excellent foundation. Evolve to CQRS when traffic patterns demand it—the investment in clean architecture makes that evolution straightforward.

---

## References & Further Reading

- CQRS Pattern: https://martinfowler.com/bliki/CQRS.html
- Event Sourcing: https://martinfowler.com/eaaDev/EventSourcing.html
- Clean Architecture: _Clean Architecture_ by Robert C. Martin
- Domain-Driven Design: _Domain-Driven Design_ by Eric Evans
