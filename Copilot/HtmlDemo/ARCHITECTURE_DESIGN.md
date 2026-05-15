# Order Management System - Technical Design

## ASP.NET Core 8.0 with Clean Architecture

---

## 1. ARCHITECTURE OVERVIEW

```
┌─────────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER                         │
│        (REST API Controllers, API Endpoints, DTOs)           │
├─────────────────────────────────────────────────────────────┤
│                   APPLICATION LAYER                          │
│     (Use Cases, Business Logic, Services, Validators)        │
├─────────────────────────────────────────────────────────────┤
│                    DOMAIN LAYER                              │
│         (Entities, Value Objects, Interfaces)                │
├─────────────────────────────────────────────────────────────┤
│                 INFRASTRUCTURE LAYER                         │
│   (Data Access, Payment Gateway, Notifications, Logging)     │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. PROJECT STRUCTURE

```
OrderManagementSystem/
├── OMS.Api/                           # Presentation Layer
│   ├── Controllers/
│   │   ├── OrderController.cs
│   │   ├── ProductController.cs
│   │   ├── PaymentController.cs
│   │   └── InventoryController.cs
│   ├── DTOs/
│   │   ├── CreateOrderDto.cs
│   │   ├── OrderResponseDto.cs
│   │   ├── PaymentDto.cs
│   │   └── InventoryCheckDto.cs
│   ├── Middleware/
│   │   ├── ErrorHandlingMiddleware.cs
│   │   └── AuthenticationMiddleware.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── OMS.Application/                   # Application Layer
│   ├── UseCases/
│   │   ├── Orders/
│   │   │   ├── CreateOrderUseCase.cs
│   │   │   ├── GetOrderUseCase.cs
│   │   │   └── CancelOrderUseCase.cs
│   │   ├── Payments/
│   │   │   ├── ProcessPaymentUseCase.cs
│   │   │   └── RefundPaymentUseCase.cs
│   │   └── Inventory/
│   │       ├── CheckInventoryUseCase.cs
│   │       └── ReserveInventoryUseCase.cs
│   ├── Services/
│   │   ├── OrderService.cs
│   │   ├── InventoryService.cs
│   │   ├── PaymentService.cs
│   │   └── NotificationService.cs
│   ├── Validators/
│   │   ├── CreateOrderValidator.cs
│   │   ├── PaymentValidator.cs
│   │   └── InventoryValidator.cs
│   ├── Interfaces/
│   │   ├── IOrderService.cs
│   │   ├── IPaymentProcessor.cs
│   │   └── IInventoryValidator.cs
│   └── Mapping/
│       └── MappingProfile.cs
│
├── OMS.Domain/                        # Domain Layer
│   ├── Entities/
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── Product.cs
│   │   ├── Inventory.cs
│   │   ├── Payment.cs
│   │   ├── Customer.cs
│   │   └── PaymentMethod.cs
│   ├── ValueObjects/
│   │   ├── Money.cs
│   │   ├── OrderStatus.cs
│   │   ├── PaymentStatus.cs
│   │   ├── Address.cs
│   │   └── OrderId.cs
│   ├── Specifications/
│   │   ├── OrderSpecifications.cs
│   │   └── InventorySpecifications.cs
│   ├── Interfaces/
│   │   ├── IOrderRepository.cs
│   │   ├── IInventoryRepository.cs
│   │   ├── IProductRepository.cs
│   │   ├── IPaymentRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   └── IDomainEvent.cs
│   └── DomainEvents/
│       ├── OrderCreatedEvent.cs
│       ├── OrderPaymentProcessedEvent.cs
│       └── InventoryReservedEvent.cs
│
├── OMS.Infrastructure/                # Infrastructure Layer
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── OrderRepository.cs
│   │   │   ├── InventoryRepository.cs
│   │   │   ├── ProductRepository.cs
│   │   │   └── PaymentRepository.cs
│   │   └── Migrations/
│   ├── ExternalServices/
│   │   ├── PaymentGateways/
│   │   │   ├── IPaymentGateway.cs
│   │   │   ├── StripePaymentGateway.cs
│   │   │   └── PayPalPaymentGateway.cs
│   │   ├── InventoryServices/
│   │   │   └── InventoryApiClient.cs
│   │   └── NotificationServices/
│   │       ├── INotificationService.cs
│   │       ├── EmailNotificationService.cs
│   │       └── SmsNotificationService.cs
│   ├── Configuration/
│   │   ├── PaymentSettings.cs
│   │   └── InventorySettings.cs
│   └── DependencyInjection.cs
│
└── OMS.Tests/                         # Testing Layer
    ├── Unit/
    │   ├── OrderServiceTests.cs
    │   ├── PaymentServiceTests.cs
    │   └── InventoryValidatorTests.cs
    ├── Integration/
    │   ├── OrderControllerTests.cs
    │   └── PaymentProcessingTests.cs
    └── TestFixtures/
        └── TestData.cs
```

---

## 3. CORE LAYERS & RESPONSIBILITIES

### 3.1 DOMAIN LAYER (OMS.Domain)

**Responsibility**: Business rules, entities, and value objects

#### Key Entities:

**Order Entity**

```csharp
public class Order : AggregateRoot
{
    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; }
    public Payment Payment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
}
```

**OrderItem Entity**

```csharp
public class OrderItem : ValueObject
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }
}
```

**Inventory Entity**

```csharp
public class Inventory : AggregateRoot
{
    public InventoryId Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public DateTime LastRestockDate { get; private set; }
}
```

**Payment Entity**

```csharp
public class Payment : AggregateRoot
{
    public PaymentId Id { get; private set; }
    public OrderId OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string TransactionId { get; private set; }
    public DateTime ProcessedAt { get; private set; }
}
```

#### Value Objects:

- `OrderId`, `OrderStatus` (Pending, Processing, Completed, Cancelled)
- `PaymentStatus` (Pending, Processing, Success, Failed, Refunded)
- `Money` (Amount, Currency)
- `Address` (Street, City, State, ZipCode, Country)

---

### 3.2 APPLICATION LAYER (OMS.Application)

**Responsibility**: Orchestrates use cases, validates input, coordinates between domain and infrastructure

#### Use Cases:

**1. Create Order Use Case**

```
Input: CreateOrderRequest (CustomerId, Items, ShippingAddress, PaymentMethod)
Process:
  → Validate customer exists
  → Validate product availability & pricing
  → Check inventory against requested quantities
  → Reserve inventory for order
  → Create Order aggregate
  → Publish OrderCreated event
  → Trigger payment processing
Output: OrderResponse (OrderId, Status, Total)
```

**2. Process Payment Use Case**

```
Input: PaymentRequest (OrderId, Amount, PaymentMethod)
Process:
  → Validate payment method
  → Call external payment gateway
  → Handle payment response
  → If Success: Update Payment status, Mark order as Processing
  → If Failed: Trigger refund, Release reserved inventory
  → Publish PaymentProcessed event
  → Send confirmation notification
Output: PaymentResponse (TransactionId, Status)
```

**3. Validate Inventory Use Case**

```
Input: InventoryCheckRequest (ProductId, Quantity)
Process:
  → Check current inventory level
  → Calculate available = Total - Reserved
  → Return availability status
Output: InventoryAvailability (Available, Quantity, ReestockDate)
```

**4. Reserve Inventory Use Case**

```
Input: ReservationRequest (OrderId, Items)
Process:
  → For each OrderItem:
    → Lock inventory record
    → Check availability
    → Reserve quantity
  → On Success: Publish InventoryReserved event
  → On Failure: Compensate (Release already reserved items)
Output: ReservationResult (Success, ReservedItems, Timestamp)
```

#### Services:

**OrderService**

- CreateOrder()
- GetOrder()
- CancelOrder()
- UpdateOrderStatus()

**PaymentService**

- ProcessPayment()
- RefundPayment()
- ValidatePaymentMethod()

**InventoryService**

- CheckAvailability()
- ReserveInventory()
- ReleaseReservation()
- UpdateStock()

---

### 3.3 INFRASTRUCTURE LAYER (OMS.Infrastructure)

**Responsibility**: Data persistence, external integrations, configurations

#### Repositories:

**IOrderRepository**

- Add(Order)
- GetById(OrderId)
- Update(Order)
- Delete(OrderId)
- GetPending Orders()

**IInventoryRepository**

- GetByProductId(ProductId)
- Update(Inventory)
- LockForUpdate(ProductId)

**IPaymentRepository**

- Add(Payment)
- GetByOrderId(OrderId)
- Update(Payment)

#### External Services:

**Payment Gateway Integration**

```csharp
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken);

    Task<RefundResult> RefundAsync(
        string transactionId,
        Money amount,
        CancellationToken cancellationToken);

    Task<PaymentStatus> GetPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken);
}
```

**Inventory API Client**

```csharp
public interface IInventoryApiClient
{
    Task<InventoryLevel> GetAvailabilityAsync(
        ProductId productId,
        CancellationToken cancellationToken);

    Task<bool> ReserveAsync(
        ProductId productId,
        int quantity,
        CancellationToken cancellationToken);
}
```

**Notification Service**

```csharp
public interface INotificationService
{
    Task SendOrderConfirmationAsync(OrderId orderId);
    Task SendPaymentFailureNotificationAsync(OrderId orderId);
    Task SendShippingNotificationAsync(OrderId orderId);
}
```

#### Database Context:

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
}
```

---

### 3.4 PRESENTATION LAYER (OMS.Api)

**Responsibility**: API endpoints, request/response handling, input validation

#### Controllers:

**OrderController**

```
POST   /api/orders                    → Create order
GET    /api/orders/{orderId}          → Get order details
GET    /api/orders                    → List orders
PATCH  /api/orders/{orderId}/cancel   → Cancel order
```

**PaymentController**

```
POST   /api/payments/process          → Process payment
POST   /api/payments/{paymentId}/refund → Refund payment
GET    /api/payments/{paymentId}      → Get payment status
```

**InventoryController**

```
GET    /api/inventory/{productId}     → Check inventory
POST   /api/inventory/check-batch     → Check multiple products
```

#### DTOs (Data Transfer Objects):

```csharp
public class CreateOrderDto
{
    public string CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public AddressDto ShippingAddress { get; set; }
    public PaymentMethodDto PaymentMethod { get; set; }
}

public class OrderResponseDto
{
    public string OrderId { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PaymentDto
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string CurrencyCode { get; set; }
}
```

---

## 4. KEY WORKFLOWS

### 4.1 Order Creation Flow

```
1. Customer submits order via API
2. OrderController validates input (DTO validation)
3. CreateOrderUseCase executes:
   ├─ Validate customer exists
   ├─ Fetch product details & prices
   ├─ Call CheckInventoryUseCase
   │  └─ InventoryService validates availability
   ├─ Call ReserveInventoryUseCase
   │  └─ Inventory locked & reserved (Pessimistic locking)
   ├─ Create Order aggregate (Domain logic)
   ├─ Persist Order via OrderRepository
   ├─ Publish OrderCreated event
   └─ Return OrderId
4. Trigger ProcessPaymentUseCase
5. Return CreateOrderResponse (201)
```

### 4.2 Payment Processing Flow

```
1. PaymentController receives payment request
2. ProcessPaymentUseCase executes:
   ├─ Validate payment data
   ├─ Fetch Payment entity
   ├─ Call external payment gateway (Stripe/PayPal)
   │  └─ IPaymentGateway.ProcessPaymentAsync()
   ├─ Handle gateway response:
   │  ├─ SUCCESS:
   │  │  ├─ Update Payment.Status = Success
   │  │  ├─ Update Order.Status = Processing
   │  │  ├─ Persist Payment & Order
   │  │  ├─ Publish OrderPaymentProcessed event
   │  │  └─ Send confirmation notification
   │  └─ FAILURE:
   │     ├─ Update Payment.Status = Failed
   │     ├─ Call ReleaseInventoryReservation
   │     ├─ Update Order.Status = Cancelled
   │     └─ Send failure notification
   └─ Return PaymentResponse
```

### 4.3 Inventory Validation Flow

```
1. InventoryController receives check request
2. CheckInventoryUseCase executes:
   ├─ Fetch Inventory by ProductId
   ├─ Calculate: Available = Total - Reserved
   ├─ Compare with requested quantity
   ├─ Return:
   │  ├─ Available (quantity available)
   │  ├─ PartiallyAvailable (some requested qty available)
   │  └─ OutOfStock (no availability)
   └─ Include restock estimate
```

---

## 5. DESIGN PATTERNS & PRINCIPLES

### 5.1 Design Patterns

| Pattern               | Usage                   | Benefit                 |
| --------------------- | ----------------------- | ----------------------- |
| Repository Pattern    | Data access abstraction | Testability, decoupling |
| Unit of Work          | Transaction management  | Consistency, atomicity  |
| Specification Pattern | Complex queries         | Reusable query logic    |
| Value Objects         | Domain modeling         | Type safety, invariants |
| Aggregate Pattern     | Entity grouping         | Bounded contexts        |
| Mediator Pattern      | Use case orchestration  | Separation of concerns  |
| Dependency Injection  | Service registration    | Loose coupling          |
| Factory Pattern       | Entity creation         | Encapsulation           |
| Observer Pattern      | Event handling          | Decoupled notifications |

### 5.2 SOLID Principles

- **S**ingle Responsibility: Each service/class has one reason to change
- **O**pen/Closed: Open for extension, closed for modification
- **L**iskov Substitution: Implementations interchangeable
- **I**nterface Segregation: Specific interfaces, not fat ones
- **D**ependency Inversion: Depend on abstractions, not concretions

---

## 6. DATA MODEL

```sql
-- Orders Table
CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL,
    ShippingAddress_Street NVARCHAR(200),
    ShippingAddress_City NVARCHAR(100),
    ShippingAddress_State NVARCHAR(50),
    ShippingAddress_ZipCode NVARCHAR(20),
    ShippingAddress_Country NVARCHAR(100),
    CreatedAt DATETIME NOT NULL,
    CompletedAt DATETIME,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

-- OrderItems Table
CREATE TABLE OrderItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Inventory Table (with Pessimistic Locking)
CREATE TABLE Inventory (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    AvailableQuantity INT NOT NULL,
    ReservedQuantity INT NOT NULL,
    LastRestockDate DATETIME NOT NULL,
    Version INT NOT NULL DEFAULT 0, -- Optimistic locking
    LockedUntil DATETIME, -- Pessimistic locking
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Payments Table
CREATE TABLE Payments (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    PaymentMethodType NVARCHAR(50) NOT NULL,
    TransactionId NVARCHAR(200),
    ProcessedAt DATETIME,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);

-- Products Table
CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL,
    Sku NVARCHAR(100) UNIQUE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL
);

-- Customers Table
CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(200) UNIQUE NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    CreatedAt DATETIME NOT NULL
);
```

---

## 7. CROSS-CUTTING CONCERNS

### 7.1 Error Handling

- Global exception middleware
- Custom exceptions (OrderNotFoundException, InsufficientInventoryException)
- Structured error responses with correlation IDs

### 7.2 Logging

- Serilog integration
- Structured logging for order/payment events
- Audit trail for financial transactions

### 7.3 Security

- JWT Bearer authentication
- Role-based authorization (Customer, Admin)
- Input validation at DTO level
- Output sanitization

### 7.4 Performance

- Caching strategy for product catalog
- Async/await throughout
- Database indexing on frequently queried fields
- Connection pooling

### 7.5 Resilience

- Retry policies for payment gateway (Polly)
- Circuit breaker for external services
- Timeout handling
- Compensating transactions for failures

---

## 8. TECHNOLOGY STACK

| Layer             | Technology                               |
| ----------------- | ---------------------------------------- |
| Framework         | ASP.NET Core 8.0                         |
| Database          | SQL Server / PostgreSQL                  |
| ORM               | Entity Framework Core 8.0                |
| API               | REST with OpenAPI/Swagger                |
| Authentication    | JWT Bearer Tokens                        |
| Validation        | FluentValidation                         |
| Mapping           | AutoMapper                               |
| Logging           | Serilog                                  |
| DI Container      | Microsoft.Extensions.DependencyInjection |
| Testing           | xUnit, Moq, FluentAssertions             |
| Payment Gateway   | Stripe SDK / PayPal SDK                  |
| Resilience        | Polly                                    |
| API Documentation | Swashbuckle                              |

---

## 9. DEPLOYMENT & CONFIGURATION

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "PaymentGateway": {
    "Provider": "Stripe",
    "ApiKey": "...",
    "Timeout": 30
  },
  "InventoryService": {
    "BaseUrl": "...",
    "ApiKey": "..."
  },
  "Notifications": {
    "EmailProvider": "SendGrid",
    "SmsProvider": "Twilio"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

---

## 10. TESTING STRATEGY

### Unit Tests

- Service business logic
- Validator rules
- Domain entity invariants

### Integration Tests

- OrderController endpoints
- Payment processing workflow
- Database operations via repositories

### Test Pyramid

```
        △ (Few)
       / \  E2E Tests
      /   \
     /─────\
    /       \  Integration Tests
   /─────────\
  /           \  Unit Tests (Many)
 /─────────────\
```

---

## 11. KEY RESPONSIBILITIES MATRIX

| Component        | Create Order            | Validate Inventory | Process Payment     |
| ---------------- | ----------------------- | ------------------ | ------------------- |
| OrderController  | Route & validate input  | -                  | -                   |
| OrderService     | Coordinate flow         | -                  | -                   |
| InventoryService | -                       | Check availability | Reserve items       |
| PaymentService   | -                       | -                  | Call gateway        |
| PaymentGateway   | -                       | -                  | Execute transaction |
| Order Aggregate  | Validate business rules | -                  | -                   |
| OrderRepository  | Persist order           | -                  | -                   |
| Database         | Store data              | Lock inventory     | Store payment       |

---

## 12. IMPLEMENTATION PHASES

### Phase 1: Foundation (Week 1-2)

- [ ] Project structure & DI setup
- [ ] Domain entities & value objects
- [ ] Entity Framework migrations
- [ ] Repository pattern implementation

### Phase 2: Order Management (Week 3-4)

- [ ] Order use cases
- [ ] Order API endpoints
- [ ] Order validation
- [ ] Order services

### Phase 3: Inventory Management (Week 5-6)

- [ ] Inventory entities & repositories
- [ ] Inventory validation logic
- [ ] Reservation mechanism
- [ ] Inventory API endpoints

### Phase 4: Payment Processing (Week 7-8)

- [ ] Payment entity & repositories
- [ ] Payment gateway integration
- [ ] Payment service implementation
- [ ] Payment API endpoints

### Phase 5: Integration & Testing (Week 9-10)

- [ ] End-to-end workflow testing
- [ ] Integration tests
- [ ] Error handling & resilience
- [ ] Performance testing

### Phase 6: Production Readiness (Week 11-12)

- [ ] Logging & monitoring
- [ ] Security audit
- [ ] Documentation
- [ ] Deployment automation

---

## 13. SCALABILITY CONSIDERATIONS

### 13.1 Horizontal Scaling Strategy

**Load Balancing**

```
┌────────────────────────────────────────────────┐
│         Azure Load Balancer / AWS ELB          │
└──────────────────┬───────────────────────────┘
                   │
        ┌──────────┼──────────┐
        │          │          │
        ▼          ▼          ▼
   ┌────────┐ ┌────────┐ ┌────────┐
   │API-1   │ │API-2   │ │API-3   │
   │Instance│ │Instance│ │Instance│
   └────┬───┘ └────┬───┘ └────┬───┘
        │          │          │
        └──────────┼──────────┘
                   │
        ┌──────────▼──────────┐
        │   Connection Pool   │
        │  (Max: 100 conn)    │
        └─────────────────────┘
                   │
        ┌──────────▼──────────┐
        │   SQL Database      │
        │  (Read Replicas)    │
        └─────────────────────┘
```

**Database Read Replicas**

- Master: Write operations (Orders, Payments)
- Replica 1: Read operations (GetOrder, ListOrders, CheckInventory)
- Replica 2: Reporting and analytics
- Data replication delay: <1 second

### 13.2 Caching Strategy

**Multi-Layer Cache**

```csharp
// Layer 1: In-Memory Cache (IMemoryCache)
Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100 MB
});

// Layer 2: Distributed Cache (Redis)
Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Configuration.GetConnectionString("Redis");
    options.InstanceName = "oms_";
});

// Cache Keys & TTL
PRODUCT_CACHE_KEY: "products:{productId}" → TTL: 24 hours
INVENTORY_CACHE_KEY: "inventory:{productId}" → TTL: 5 minutes (invalidated on update)
CUSTOMER_CACHE_KEY: "customer:{customerId}" → TTL: 12 hours
ORDER_CACHE_KEY: "order:{orderId}" → TTL: 1 hour (invalidated on update)
```

**Cache Invalidation Pattern**

```csharp
public class OrderService
{
    private readonly IDistributedCache _cache;

    public async Task UpdateOrderAsync(Order order)
    {
        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Invalidate cache on update
        var cacheKey = $"order:{order.Id.Value}";
        await _cache.RemoveAsync(cacheKey);
    }
}
```

### 13.3 Partitioning Strategy

**Database Sharding by Region**

```
Region: US East
├─ Orders DB (Shard 1)
├─ Customers: A-M
└─ Orders: Q1-Q4 2026

Region: US West
├─ Orders DB (Shard 2)
├─ Customers: N-Z
└─ Orders: Replicated

Region: EU
├─ Orders DB (Shard 3)
├─ Customers: EU-specific
└─ Orders: Compliant storage
```

**Shard Key Selection**

- Primary: `CustomerId` (ensures customer data locality)
- Secondary: `CreatedAt` (time-based partitioning for old data archival)

### 13.4 Queue-Based Async Processing

**Message Queue Architecture**

```
┌─────────────────────────────┐
│  Order Created Event        │
│  (Synchronous Endpoint)     │
└──────────────┬──────────────┘
               │
               ▼
    ┌──────────────────────┐
    │  Azure Service Bus   │
    │  / RabbitMQ Queue    │
    └──────────────────────┘
               │
     ┌─────────┼─────────┐
     ▼         ▼         ▼
  Email    SMS       Analytics
  Worker   Worker    Worker
```

**Queue Names & Routing**

- `orders.created` → Email notification worker
- `orders.payment-processed` → Order fulfillment worker
- `orders.failed` → Alerts & escalation
- `inventory.reserved` → Analytics processor

### 13.5 Auto-Scaling Policies

**CPU-Based Scaling**

- Minimum instances: 3
- Maximum instances: 10
- Target CPU: 70%
- Scale-up threshold: >80% for 2 minutes
- Scale-down threshold: <30% for 10 minutes

**Request-Based Scaling**

- Target: 1000 requests/sec per instance
- Burst handling: 1500 req/sec (temporary)

**Database Scaling**

- Connection pool: 100 connections
- Timeout: 30 seconds
- Auto-grow storage: Up to 1TB

---

## 14. ASYNC PROCESSING ARCHITECTURE

### 14.1 Async/Await Throughout

**All I/O Operations are Async**

```csharp
// Database operations
public async Task<Order> GetOrderAsync(OrderId id, CancellationToken ct)
{
    return await _orderRepository.GetByIdAsync(id, ct);
}

// HTTP calls to external services
public async Task<PaymentResult> ProcessPaymentAsync(
    PaymentRequest request,
    CancellationToken ct)
{
    return await _paymentGateway.ProcessPaymentAsync(request, ct);
}

// Cache operations
public async Task<Product> GetProductAsync(ProductId id, CancellationToken ct)
{
    var cacheKey = $"product:{id.Value}";
    var cached = await _cache.GetStringAsync(cacheKey, ct);
    if (!string.IsNullOrEmpty(cached))
        return JsonSerializer.Deserialize<Product>(cached);

    var product = await _productRepository.GetByIdAsync(id, ct);
    await _cache.SetStringAsync(cacheKey,
        JsonSerializer.Serialize(product),
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) },
        ct);
    return product;
}
```

### 14.2 Background Job Processing (Hangfire)

**Long-Running Operations Queue**

```csharp
public class OrderBackgroundJobs
{
    private readonly IOrderService _orderService;
    private readonly INotificationService _notificationService;

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessOrderFulfillmentAsync(
        Guid orderId,
        IJobCancellationToken cancellationToken)
    {
        try
        {
            // Simulate long-running fulfillment process
            await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken.ShutdownToken);

            await _orderService.MarkOrderAsShippedAsync(
                OrderId.Create(orderId),
                cancellationToken.ShutdownToken);

            await _notificationService.SendShippingNotificationAsync(
                OrderId.Create(orderId));
        }
        catch (Exception ex)
        {
            // Log and re-throw for Hangfire retry
            throw new InvalidOperationException($"Order fulfillment failed for {orderId}", ex);
        }
    }
}

// Enqueue job
BackgroundJob.Enqueue<OrderBackgroundJobs>(
    x => x.ProcessOrderFulfillmentAsync(orderId, JobCancellationToken.Null));
```

### 14.3 Real-Time Notifications via SignalR

**Payment Status Updates**

```csharp
public class PaymentHub : Hub
{
    private readonly IPaymentService _paymentService;

    public async Task SubscribeToOrderPaymentAsync(string orderId)
    {
        // Add client to group for this order
        await Groups.AddToGroupAsync(Connection.ConnectionId, $"order-{orderId}");
    }
}

public class PaymentNotificationService
{
    private readonly IHubContext<PaymentHub> _hubContext;

    public async Task NotifyPaymentCompleted(
        OrderId orderId,
        PaymentStatus status,
        CancellationToken ct)
    {
        await _hubContext.Clients
            .Group($"order-{orderId.Value}")
            .SendAsync("PaymentStatusUpdated", new
            {
                Status = status.Value,
                UpdatedAt = DateTime.UtcNow,
                Message = "Your payment has been processed successfully"
            }, ct);
    }
}
```

### 14.4 Scheduled Tasks (Quartz.NET)

**Periodic Background Jobs**

```csharp
public class InventoryReconciliationJob : IJob
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryReconciliationJob> _logger;

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Starting inventory reconciliation");

            // Reconcile inventory with external system
            await _inventoryService.ReconcileWithExternalSystemAsync(
                context.CancellationToken);

            _logger.LogInformation("Inventory reconciliation completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Inventory reconciliation failed");
            throw;
        }
    }
}

// Schedule in Program.cs
services.AddQuartz(q =>
{
    q.AddJob<InventoryReconciliationJob>(opts =>
        opts.WithIdentity("inventory-reconciliation"));

    q.AddTrigger(opts => opts
        .ForJob("inventory-reconciliation")
        .WithIdentity("inventory-reconciliation-trigger")
        .WithCronSchedule("0 0 * * * ?") // Every hour
        .Build());
});
```

### 14.5 Async Patterns for Common Operations

**Fire-and-Forget Pattern (with tracking)**

```csharp
public async Task<CreateOrderResponse> CreateOrderAsync(
    CreateOrderRequest request,
    CancellationToken ct)
{
    // Create and persist order (blocking)
    var order = Order.Create(...);
    await _orderRepository.AddAsync(order, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    // Send notifications asynchronously (non-blocking)
    _ = _notificationService.SendOrderConfirmationAsync(order.Id);

    // Schedule payment processing
    BackgroundJob.Enqueue<PaymentService>(
        x => x.ProcessPaymentAsync(order.Id, null));

    return new CreateOrderResponse(order.Id.Value, order.Status.Value, order.TotalAmount.Amount);
}
```

**Batch Async Processing**

```csharp
public async Task<List<InventoryCheckResult>> CheckInventoryBatchAsync(
    List<ProductId> productIds,
    CancellationToken ct)
{
    // Process multiple inventory checks in parallel
    var tasks = productIds.Select(productId =>
        _inventoryService.CheckAvailabilityAsync(productId, ct));

    var results = await Task.WhenAll(tasks);
    return results.ToList();
}
```

---

## 15. PAYMENT API INTEGRATION DETAILS

### 15.1 Stripe Integration (Primary Gateway)

**Stripe Configuration**

```json
{
  "Stripe": {
    "PublicKey": "pk_live_...",
    "SecretKey": "sk_live_...",
    "WebhookSecret": "whsec_...",
    "IdempotencyKey": "idempotency-{orderId}-{timestamp}",
    "TimeoutSeconds": 30,
    "RetryMaxAttempts": 3
  }
}
```

**Complete Stripe Implementation**

```csharp
public class StripePaymentGateway : IPaymentGateway
{
    private readonly StripeClient _client;
    private readonly ILogger<StripePaymentGateway> _logger;
    private readonly IPollyPolicies _pollyPolicies;
    private readonly string _idempotencyKeyPrefix;

    public StripePaymentGateway(
        IOptions<StripeSettings> options,
        ILogger<StripePaymentGateway> logger,
        IPollyPolicies pollyPolicies)
    {
        StripeConfiguration.ApiKey = options.Value.SecretKey;
        StripeConfiguration.ApiVersion = "2023-10-16";
        _logger = logger;
        _pollyPolicies = pollyPolicies;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken)
    {
        using var activity = new Activity("StripePaymentGateway.ProcessPayment").Start();

        try
        {
            _logger.LogInformation(
                "Processing Stripe payment for order {OrderId}, amount {Amount} {Currency}",
                request.OrderId, request.Amount, request.Currency);

            // Use idempotency key to prevent duplicate charges
            var idempotencyKey = $"order-{request.OrderId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = ConvertToSmallestCurrencyUnit(request.Amount),
                Currency = request.Currency.ToLower(),
                PaymentMethod = request.PaymentToken,
                Confirm = true,
                ReturnUrl = "https://yourdomain.com/payment-confirmation",
                Description = $"Order {request.OrderId}",
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", request.OrderId.ToString() },
                    { "customer_id", request.CustomerId?.ToString() },
                    { "timestamp", DateTime.UtcNow.ToString("O") }
                },
                OffSession = false,
                StatementDescriptor = "OMS Order",
                ReceiptEmail = request.CustomerEmail
            };

            // Execute with retry policy (exponential backoff)
            var paymentIntent = await _pollyPolicies.GetRetryPolicy<PaymentIntent>()
                .ExecuteAsync(async () =>
                {
                    var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
                    var service = new PaymentIntentService();
                    return await service.CreateAsync(paymentIntentOptions, requestOptions, cancellationToken);
                });

            // Handle different payment statuses
            return HandlePaymentIntentStatus(paymentIntent);
        }
        catch (StripeException ex) when (ex.Error?.Code == "rate_limit")
        {
            _logger.LogWarning(ex, "Stripe rate limit exceeded, retrying...");
            throw new StripeRateLimitException("Too many requests to Stripe", ex);
        }
        catch (StripeException ex) when (ex.Error?.Code == "card_declined")
        {
            _logger.LogInformation(ex, "Card declined for order {OrderId}", request.OrderId);
            return PaymentResult.Failure($"Card declined: {ex.Error.DeclineCode}");
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error processing payment for order {OrderId}", request.OrderId);
            return PaymentResult.Failure($"Payment gateway error: {ex.Error?.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing Stripe payment");
            throw;
        }
    }

    public async Task<RefundResult> RefundAsync(
        string transactionId,
        Money amount,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing refund for transaction {TransactionId}, amount {Amount} {Currency}",
                transactionId, amount.Amount, amount.Currency);

            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = transactionId,
                Amount = ConvertToSmallestCurrencyUnit(amount.Amount),
                Reason = RefundReasons.RequestedByCustomer,
                Metadata = new Dictionary<string, string>
                {
                    { "refund_timestamp", DateTime.UtcNow.ToString("O") }
                }
            };

            var service = new RefundService();
            var refund = await service.CreateAsync(refundOptions, null, cancellationToken);

            if (refund.Status == "succeeded")
            {
                _logger.LogInformation("Refund successful: {RefundId}", refund.Id);
                return RefundResult.Success(refund.Id);
            }
            else
            {
                _logger.LogWarning("Refund not succeeded. Status: {Status}", refund.Status);
                return RefundResult.Failure($"Refund status: {refund.Status}");
            }
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe refund error for transaction {TransactionId}", transactionId);
            return RefundResult.Failure($"Refund failed: {ex.Error?.Message}");
        }
    }

    public async Task<PaymentStatus> GetPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(transactionId, null, cancellationToken);

            return MapStripeStatusToPaymentStatus(paymentIntent.Status);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error retrieving payment status for {TransactionId}", transactionId);
            throw;
        }
    }

    private PaymentResult HandlePaymentIntentStatus(PaymentIntent paymentIntent)
    {
        return paymentIntent.Status switch
        {
            "succeeded" => PaymentResult.Success(paymentIntent.Id),
            "processing" => PaymentResult.Success(paymentIntent.Id), // Will complete soon
            "requires_payment_method" => PaymentResult.Failure("Payment method required"),
            "requires_action" => PaymentResult.Failure("Additional authentication required"),
            _ => PaymentResult.Failure($"Unexpected status: {paymentIntent.Status}")
        };
    }

    private PaymentStatus MapStripeStatusToPaymentStatus(string stripeStatus)
    {
        return stripeStatus switch
        {
            "succeeded" => PaymentStatus.Success,
            "processing" => PaymentStatus.Processing,
            "requires_payment_method" => PaymentStatus.Failed,
            _ => PaymentStatus.Pending
        };
    }

    private long ConvertToSmallestCurrencyUnit(decimal amount)
    {
        // Most currencies use 2 decimal places (cents)
        return (long)(amount * 100);
    }
}
```

### 15.2 PayPal Integration (Secondary Gateway)

**PayPal Configuration**

```json
{
  "PayPal": {
    "ClientId": "...",
    "ClientSecret": "...",
    "Mode": "Live",
    "WebhookId": "...",
    "TimeoutSeconds": 30
  }
}
```

**PayPal Implementation**

```csharp
public class PayPalPaymentGateway : IPaymentGateway
{
    private readonly PayPalHttpClient _client;
    private readonly ILogger<PayPalPaymentGateway> _logger;

    public async Task<PaymentResult> ProcessPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing PayPal payment for order {OrderId}", request.OrderId);

            // Create PayPal order
            var createOrderRequest = new CreateOrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = request.Currency,
                            Value = request.Amount.ToString("F2")
                        },
                        ReferenceId = request.OrderId.ToString()
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "https://yourdomain.com/payment-success",
                    CancelUrl = "https://yourdomain.com/payment-cancelled"
                }
            };

            // Execute with timeout
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var createOrderResponse = await _client.Execute(createOrderRequest, cts.Token);

            if (createOrderResponse.StatusCode == 201 && createOrderResponse.Result is Order order)
            {
                _logger.LogInformation("PayPal order created: {OrderId}", order.Id);
                return PaymentResult.Success(order.Id);
            }

            return PaymentResult.Failure("Failed to create PayPal order");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "PayPal HTTP error");
            return PaymentResult.Failure("Payment gateway unavailable");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayPal payment processing error");
            return PaymentResult.Failure($"Payment error: {ex.Message}");
        }
    }

    public async Task<RefundResult> RefundAsync(
        string transactionId,
        Money amount,
        CancellationToken cancellationToken)
    {
        // Similar implementation with PayPal refund API
        throw new NotImplementedException();
    }

    public async Task<PaymentStatus> GetPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken)
    {
        // Query PayPal for transaction status
        throw new NotImplementedException();
    }
}
```

### 15.3 Webhook Handling for Payment Callbacks

**Webhook Processing**

```csharp
[ApiController]
[Route("api/[controller]")]
public class PaymentWebhookController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentWebhookController> _logger;

    [HttpPost("stripe")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleStripeWebhook(
        [FromBody] string json,
        CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            _webhookSecret);

        try
        {
            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogInformation(
                        "Payment succeeded: {PaymentIntentId}", paymentIntent.Id);
                    await _paymentService.ConfirmPaymentAsync(
                        paymentIntent.Id, cancellationToken);
                    break;

                case Events.PaymentIntentPaymentFailed:
                    var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogWarning(
                        "Payment failed: {PaymentIntentId}", failedPaymentIntent.Id);
                    await _paymentService.FailPaymentAsync(
                        failedPaymentIntent.Id, cancellationToken);
                    break;

                case Events.ChargeRefunded:
                    var charge = stripeEvent.Data.Object as Charge;
                    _logger.LogInformation(
                        "Charge refunded: {ChargeId}", charge.Id);
                    await _paymentService.ProcessRefundAsync(
                        charge.Id, charge.Amount, cancellationToken);
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return BadRequest();
        }
    }
}
```

### 15.4 Payment Retry Policy with Circuit Breaker

**Polly Configuration**

```csharp
public class ResiliencePolicies
{
    public static IAsyncPolicy<T> GetPaymentRetryPolicy<T>() where T : class
    {
        return Policy.Handle<StripeException>()
            .Or<HttpRequestException>()
            .OrResult<T>(r => r == null)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryAttempt - 1)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"Retry {retryCount} after {timespan.TotalMilliseconds}ms. "
                        + $"Reason: {outcome.Exception?.Message}");
                });
    }

    public static IAsyncPolicy<T> GetPaymentCircuitBreakerPolicy<T>() where T : class
    {
        return Policy.Handle<StripeException>()
            .OrResult<T>(r => r == null)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, durationOfBreak) =>
                {
                    Console.WriteLine(
                        $"Circuit breaker opened for {durationOfBreak.TotalSeconds}s. "
                        + $"Reason: {outcome.Exception?.Message}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                });
    }
}

// Wrap with both policies
var policy = Policy.WrapAsync(
    ResiliencePolicies.GetPaymentRetryPolicy<PaymentResult>(),
    ResiliencePolicies.GetPaymentCircuitBreakerPolicy<PaymentResult>());
```

### 15.5 Payment Reconciliation Job

**Periodic Payment Status Sync**

```csharp
public class PaymentReconciliationJob : IJob
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly ILogger<PaymentReconciliationJob> _logger;

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting payment reconciliation");

        try
        {
            // Find all pending payments older than 5 minutes
            var pendingPayments = await _paymentRepository
                .GetPendingPaymentsOlderThanAsync(
                    TimeSpan.FromMinutes(5),
                    context.CancellationToken);

            foreach (var payment in pendingPayments)
            {
                try
                {
                    // Check status with payment gateway
                    var status = await _paymentGateway.GetPaymentStatusAsync(
                        payment.TransactionId,
                        context.CancellationToken);

                    if (status != payment.Status)
                    {
                        _logger.LogInformation(
                            "Payment status changed from {OldStatus} to {NewStatus}",
                            payment.Status, status);

                        payment.UpdateStatus(status);
                        await _paymentRepository.UpdateAsync(payment, context.CancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error reconciling payment {PaymentId}",
                        payment.Id);
                }
            }

            _logger.LogInformation(
                "Payment reconciliation completed. Processed {Count} payments",
                pendingPayments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payment reconciliation job failed");
            throw;
        }
    }
}
```
