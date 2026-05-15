// ============================================================================
// ORDER MANAGEMENT SYSTEM - SKELETON CODE STRUCTURE
// ASP.NET Core 8.0 with Clean Architecture
// ============================================================================

// ============================================================================
// 1. DOMAIN LAYER - Core Business Logic
// ============================================================================

namespace OMS.Domain.Entities;

#region Aggregates & Entities

/// <summary>
/// Order Aggregate Root - Contains all related data for an order
/// </summary>
public class Order : AggregateRoot
{
public OrderId Id { get; private set; }
public CustomerId CustomerId { get; private set; }
public List<OrderItem> Items { get; private set; } = new();
public Money TotalAmount { get; private set; }
public OrderStatus Status { get; private set; }
public Address ShippingAddress { get; private set; }
public Payment? Payment { get; private set; }
public DateTime CreatedAt { get; private set; }
public DateTime? CompletedAt { get; private set; }

    // Constructor
    private Order(OrderId id, CustomerId customerId, List<OrderItem> items,
        Money totalAmount, Address shippingAddress)
    {
        Id = id;
        CustomerId = customerId;
        Items = items;
        TotalAmount = totalAmount;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OrderCreatedEvent(Id, CustomerId, TotalAmount));
    }

    // Factory method for creation
    public static Order Create(CustomerId customerId, List<OrderItem> items,
        Address shippingAddress) => new(
            OrderId.Create(),
            customerId,
            items,
            CalculateTotal(items),
            shippingAddress);

    private static Money CalculateTotal(List<OrderItem> items)
        => items.Aggregate(Money.Zero(), (acc, item) => acc.Add(item.TotalPrice));

    // Business methods
    public void ProcessPayment(PaymentStatus paymentStatus)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Order not in pending state");

        Status = paymentStatus == PaymentStatus.Success
            ? OrderStatus.Processing
            : OrderStatus.Cancelled;
    }

    public void Complete()
    {
        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        RaiseDomainEvent(new OrderCompletedEvent(Id));
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
        RaiseDomainEvent(new OrderCancelledEvent(Id));
    }

}

/// <summary>
/// OrderItem Value Object - Immutable order line item
/// </summary>
public class OrderItem : ValueObject
{
public ProductId ProductId { get; private set; }
public int Quantity { get; private set; }
public Money UnitPrice { get; private set; }
public Money TotalPrice { get; private set; }

    private OrderItem(ProductId productId, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = unitPrice.Multiply(quantity);
    }

    public static OrderItem Create(ProductId productId, int quantity, Money unitPrice)
        => new(productId, quantity, unitPrice);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return Quantity;
        yield return UnitPrice;
    }

}

/// <summary>
/// Inventory Aggregate Root - Manages product stock
/// </summary>
public class Inventory : AggregateRoot
{
public InventoryId Id { get; private set; }
public ProductId ProductId { get; private set; }
public int AvailableQuantity { get; private set; }
public int ReservedQuantity { get; private set; }
public int TotalQuantity => AvailableQuantity + ReservedQuantity;
public DateTime LastRestockDate { get; private set; }
public int Version { get; private set; } // Optimistic locking

    private Inventory(InventoryId id, ProductId productId, int quantity)
    {
        Id = id;
        ProductId = productId;
        AvailableQuantity = quantity;
        ReservedQuantity = 0;
        LastRestockDate = DateTime.UtcNow;
        Version = 0;
    }

    public static Inventory Create(ProductId productId, int quantity)
        => new(InventoryId.Create(), productId, quantity);

    // Inventory operations
    public bool CanReserve(int quantity)
        => AvailableQuantity >= quantity;

    public void Reserve(int quantity)
    {
        if (!CanReserve(quantity))
            throw new InsufficientInventoryException(
                $"Cannot reserve {quantity} units. Available: {AvailableQuantity}");

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
        Version++;
    }

    public void ReleaseReservation(int quantity)
    {
        if (ReservedQuantity < quantity)
            throw new InvalidOperationException("Invalid reservation release");

        ReservedQuantity -= quantity;
        AvailableQuantity += quantity;
        Version++;
    }

    public void Restock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Restock quantity must be positive");

        AvailableQuantity += quantity;
        LastRestockDate = DateTime.UtcNow;
        Version++;
        RaiseDomainEvent(new InventoryRestockedEvent(ProductId, quantity));
    }

}

/// <summary>
/// Payment Aggregate Root - Tracks payment transactions
/// </summary>
public class Payment : AggregateRoot
{
public PaymentId Id { get; private set; }
public OrderId OrderId { get; private set; }
public Money Amount { get; private set; }
public PaymentStatus Status { get; private set; }
public PaymentMethod PaymentMethod { get; private set; }
public string? TransactionId { get; private set; }
public DateTime ProcessedAt { get; private set; }
public DateTime CreatedAt { get; private set; }
public string? FailureReason { get; private set; }

    private Payment(PaymentId id, OrderId orderId, Money amount,
        PaymentMethod paymentMethod)
    {
        Id = id;
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Payment Create(OrderId orderId, Money amount,
        PaymentMethod paymentMethod)
        => new(PaymentId.Create(), orderId, amount, paymentMethod);

    public void MarkAsProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Invalid payment state transition");
        Status = PaymentStatus.Processing;
    }

    public void MarkAsSuccessful(string transactionId)
    {
        Status = PaymentStatus.Success;
        TransactionId = transactionId;
        ProcessedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentSucceededEvent(OrderId, Amount));
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentFailedEvent(OrderId, reason));
    }

}

#endregion

#region Value Objects

/// <summary>
/// Money Value Object - Type-safe monetary amount
/// </summary>
public class Money : ValueObject
{
public decimal Amount { get; private set; }
public string Currency { get; private set; }

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        if (string.IsNullOrEmpty(currency))
            throw new ArgumentException("Currency is required");

        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "USD")
        => new(amount, currency);

    public static Money Zero() => new(0, "USD");

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int quantity)
        => new(Amount * quantity, Currency);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

}

/// <summary>
/// Address Value Object
/// </summary>
public class Address : ValueObject
{
public string Street { get; private set; }
public string City { get; private set; }
public string State { get; private set; }
public string ZipCode { get; private set; }
public string Country { get; private set; }

    private Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    public static Address Create(string street, string city, string state,
        string zipCode, string country)
        => new(street, city, state, zipCode, country);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

}

/// <summary>
/// Strongly-typed IDs using Value Objects
/// </summary>
public class OrderId : ValueObject
{
public Guid Value { get; private set; }
private OrderId(Guid value) => Value = value;
public static OrderId Create() => new(Guid.NewGuid());
public static OrderId Create(Guid value) => new(value);
protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
}

public class CustomerId : ValueObject
{
public Guid Value { get; private set; }
private CustomerId(Guid value) => Value = value;
public static CustomerId Create(Guid value) => new(value);
protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
}

public class ProductId : ValueObject
{
public Guid Value { get; private set; }
private ProductId(Guid value) => Value = value;
public static ProductId Create(Guid value) => new(value);
protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
}

public class InventoryId : ValueObject
{
public Guid Value { get; private set; }
private InventoryId(Guid value) => Value = value;
public static InventoryId Create() => new(Guid.NewGuid());
public static InventoryId Create(Guid value) => new(value);
protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
}

public class PaymentId : ValueObject
{
public Guid Value { get; private set; }
private PaymentId(Guid value) => Value = value;
public static PaymentId Create() => new(Guid.NewGuid());
protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
}

/// <summary>
/// Order Status Enumeration as Value Object
/// </summary>
public class OrderStatus : ValueObject
{
public static readonly OrderStatus Pending = new("Pending");
public static readonly OrderStatus Processing = new("Processing");
public static readonly OrderStatus Completed = new("Completed");
public static readonly OrderStatus Cancelled = new("Cancelled");

    public string Value { get; private set; }

    private OrderStatus(string value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

}

/// <summary>
/// Payment Status Enumeration
/// </summary>
public class PaymentStatus : ValueObject
{
public static readonly PaymentStatus Pending = new("Pending");
public static readonly PaymentStatus Processing = new("Processing");
public static readonly PaymentStatus Success = new("Success");
public static readonly PaymentStatus Failed = new("Failed");
public static readonly PaymentStatus Refunded = new("Refunded");

    public string Value { get; private set; }
    private PaymentStatus(string value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

}

#endregion

#region Domain Events

/// <summary>
/// Domain event base class
/// </summary>
public abstract class DomainEvent
{
public Guid Id { get; } = Guid.NewGuid();
public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public class OrderCreatedEvent : DomainEvent
{
public OrderId OrderId { get; }
public CustomerId CustomerId { get; }
public Money TotalAmount { get; }

    public OrderCreatedEvent(OrderId orderId, CustomerId customerId, Money totalAmount)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }

}

public class OrderCompletedEvent : DomainEvent
{
public OrderId OrderId { get; }
public OrderCompletedEvent(OrderId orderId) => OrderId = orderId;
}

public class OrderCancelledEvent : DomainEvent
{
public OrderId OrderId { get; }
public OrderCancelledEvent(OrderId orderId) => OrderId = orderId;
}

public class PaymentSucceededEvent : DomainEvent
{
public OrderId OrderId { get; }
public Money Amount { get; }
public PaymentSucceededEvent(OrderId orderId, Money amount)
{
OrderId = orderId;
Amount = amount;
}
}

public class PaymentFailedEvent : DomainEvent
{
public OrderId OrderId { get; }
public string Reason { get; }
public PaymentFailedEvent(OrderId orderId, string reason)
{
OrderId = orderId;
Reason = reason;
}
}

public class InventoryRestockedEvent : DomainEvent
{
public ProductId ProductId { get; }
public int Quantity { get; }
public InventoryRestockedEvent(ProductId productId, int quantity)
{
ProductId = productId;
Quantity = quantity;
}
}

#endregion

// ============================================================================
// 2. APPLICATION LAYER - Use Cases & Services
// ============================================================================

namespace OMS.Application.UseCases.Orders;

/// <summary>
/// Create Order Use Case - Main business orchestration
/// </summary>
public class CreateOrderUseCase
{
private readonly IOrderRepository \_orderRepository;
private readonly IInventoryService \_inventoryService;
private readonly IProductRepository \_productRepository;
private readonly IUnitOfWork \_unitOfWork;

    public CreateOrderUseCase(
        IOrderRepository orderRepository,
        IInventoryService inventoryService,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _inventoryService = inventoryService;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrderResponse> ExecuteAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        // Step 1: Validate products exist and get pricing
        var products = await _productRepository.GetByIdsAsync(
            request.Items.Select(i => i.ProductId).ToList(),
            cancellationToken);

        // Step 2: Check inventory availability
        foreach (var item in request.Items)
        {
            var isAvailable = await _inventoryService.CheckAvailabilityAsync(
                item.ProductId,
                item.Quantity,
                cancellationToken);

            if (!isAvailable)
                throw new InsufficientInventoryException(
                    $"Product {item.ProductId} insufficient inventory");
        }

        // Step 3: Create order items from request
        var orderItems = request.Items.Select(item =>
        {
            var product = products.First(p => p.Id == item.ProductId);
            return OrderItem.Create(item.ProductId, item.Quantity, product.Price);
        }).ToList();

        // Step 4: Create order aggregate
        var order = Order.Create(
            CustomerId.Create(request.CustomerId),
            orderItems,
            Address.Create(request.ShippingAddress.Street, request.ShippingAddress.City,
                request.ShippingAddress.State, request.ShippingAddress.ZipCode,
                request.ShippingAddress.Country));

        // Step 5: Reserve inventory
        await _inventoryService.ReserveInventoryAsync(order.Id, orderItems, cancellationToken);

        // Step 6: Persist order
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrderResponse(order.Id.Value, order.Status.Value, order.TotalAmount.Amount);
    }

}

namespace OMS.Application.UseCases.Payments;

/// <summary>
/// Process Payment Use Case - Handles payment transactions
/// </summary>
public class ProcessPaymentUseCase
{
private readonly IPaymentRepository \_paymentRepository;
private readonly IOrderRepository \_orderRepository;
private readonly IPaymentGateway \_paymentGateway;
private readonly IInventoryService \_inventoryService;
private readonly INotificationService \_notificationService;
private readonly IUnitOfWork \_unitOfWork;

    public ProcessPaymentUseCase(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IPaymentGateway paymentGateway,
        IInventoryService inventoryService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _paymentGateway = paymentGateway;
        _inventoryService = inventoryService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcessPaymentResponse> ExecuteAsync(
        ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {
        // Step 1: Fetch order
        var order = await _orderRepository.GetByIdAsync(
            OrderId.Create(request.OrderId),
            cancellationToken);

        if (order == null)
            throw new OrderNotFoundException($"Order {request.OrderId} not found");

        // Step 2: Create payment entity
        var payment = Payment.Create(
            order.Id,
            order.TotalAmount,
            PaymentMethod.FromString(request.PaymentMethod));

        // Step 3: Mark as processing
        payment.MarkAsProcessing();

        // Step 4: Call payment gateway
        PaymentResult gatewayResult;
        try
        {
            gatewayResult = await _paymentGateway.ProcessPaymentAsync(
                new PaymentGatewayRequest(
                    order.Id.Value,
                    order.TotalAmount.Amount,
                    request.PaymentMethod,
                    request.PaymentToken),
                cancellationToken);
        }
        catch (Exception ex)
        {
            payment.MarkAsFailed(ex.Message);
            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
        }

        // Step 5: Handle gateway response
        if (gatewayResult.IsSuccessful)
        {
            payment.MarkAsSuccessful(gatewayResult.TransactionId);
            order.ProcessPayment(PaymentStatus.Success);
            await _notificationService.SendOrderConfirmationAsync(order.Id);
        }
        else
        {
            payment.MarkAsFailed(gatewayResult.ErrorMessage);
            order.ProcessPayment(PaymentStatus.Failed);

            // Compensate: Release inventory reservation
            await _inventoryService.ReleaseReservationAsync(
                order.Id,
                order.Items,
                cancellationToken);

            await _notificationService.SendPaymentFailureNotificationAsync(order.Id);
        }

        // Step 6: Persist changes
        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProcessPaymentResponse(
            payment.Id.Value,
            payment.Status.Value,
            gatewayResult.TransactionId);
    }

}

namespace OMS.Application.UseCases.Inventory;

/// <summary>
/// Check Inventory Use Case
/// </summary>
public class CheckInventoryUseCase
{
private readonly IInventoryRepository \_inventoryRepository;

    public CheckInventoryUseCase(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<InventoryAvailabilityResponse> ExecuteAsync(
        ProductId productId,
        int requestedQuantity,
        CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        if (inventory == null)
            throw new InventoryNotFoundException($"Inventory for product {productId} not found");

        var isAvailable = inventory.CanReserve(requestedQuantity);
        var availableQuantity = inventory.AvailableQuantity;

        return new InventoryAvailabilityResponse(
            productId.Value,
            isAvailable,
            availableQuantity,
            inventory.LastRestockDate);
    }

}

// ============================================================================
// 3. INFRASTRUCTURE LAYER - Repositories & External Services
// ============================================================================

namespace OMS.Infrastructure.Data.Repositories;

/// <summary>
/// Order Repository - Data access for orders
/// </summary>
public class OrderRepository : IOrderRepository
{
private readonly ApplicationDbContext \_context;

    public OrderRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        _context.Orders.Update(order);
        await Task.CompletedTask;
    }

    public async Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .Include(o => o.Items)
            .ToListAsync(cancellationToken);
    }

}

/// <summary>
/// Inventory Repository - Data access for inventory
/// </summary>
public class InventoryRepository : IInventoryRepository
{
private readonly ApplicationDbContext \_context;

    public InventoryRepository(ApplicationDbContext context) => _context = context;

    public async Task<Inventory?> GetByProductIdAsync(
        ProductId productId,
        CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken)
    {
        _context.Inventories.Update(inventory);
        await Task.CompletedTask;
    }

    public async Task<Inventory?> GetByIdWithLockAsync(
        InventoryId id,
        CancellationToken cancellationToken)
    {
        // Pessimistic locking using SELECT FOR UPDATE equivalent in EF Core
        return await _context.Inventories
            .FromSqlInterpolated($"SELECT * FROM Inventories WHERE Id = {id.Value} WITH (UPDLOCK, ROWLOCK)")
            .FirstOrDefaultAsync(cancellationToken);
    }

}

namespace OMS.Infrastructure.ExternalServices.PaymentGateways;

/// <summary>
/// Payment Gateway Interface
/// </summary>
public interface IPaymentGateway
{
Task<PaymentResult> ProcessPaymentAsync(
PaymentGatewayRequest request,
CancellationToken cancellationToken);

    Task<RefundResult> RefundAsync(
        string transactionId,
        Money amount,
        CancellationToken cancellationToken);

}

/// <summary>
/// Stripe Payment Gateway Implementation
/// </summary>
public class StripePaymentGateway : IPaymentGateway
{
private readonly IStripeClient \_stripeClient;
private readonly ILogger<StripePaymentGateway> \_logger;

    public StripePaymentGateway(
        IStripeClient stripeClient,
        ILogger<StripePaymentGateway> logger)
    {
        _stripeClient = stripeClient;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing Stripe payment for order {OrderId}", request.OrderId);

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Convert to cents
                Currency = "usd",
                PaymentMethod = request.PaymentToken,
                Confirm = true,
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", request.OrderId.ToString() }
                }
            };

            var service = new PaymentIntentService(_stripeClient);
            var paymentIntent = await service.CreateAsync(options, null, cancellationToken);

            if (paymentIntent.Status == "succeeded")
            {
                return PaymentResult.Success(paymentIntent.Id);
            }
            else if (paymentIntent.Status == "requires_action")
            {
                return PaymentResult.Failure("Additional authentication required");
            }
            else
            {
                return PaymentResult.Failure($"Payment failed: {paymentIntent.Status}");
            }
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe payment error for order {OrderId}", request.OrderId);
            return PaymentResult.Failure($"Stripe error: {ex.Message}");
        }
    }

    public async Task<RefundResult> RefundAsync(
        string transactionId,
        Money amount,
        CancellationToken cancellationToken)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = transactionId,
                Amount = (long)(amount.Amount * 100)
            };

            var service = new RefundService(_stripeClient);
            var refund = await service.CreateAsync(options, null, cancellationToken);

            return refund.Status == "succeeded"
                ? RefundResult.Success(refund.Id)
                : RefundResult.Failure("Refund failed");
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe refund error for transaction {TransactionId}", transactionId);
            return RefundResult.Failure($"Refund error: {ex.Message}");
        }
    }

}

namespace OMS.Infrastructure.ExternalServices.Notifications;

/// <summary>
/// Notification Service for order-related communications
/// </summary>
public interface INotificationService
{
Task SendOrderConfirmationAsync(OrderId orderId, CancellationToken cancellationToken = default);
Task SendPaymentFailureNotificationAsync(OrderId orderId, CancellationToken cancellationToken = default);
Task SendShippingNotificationAsync(OrderId orderId, CancellationToken cancellationToken = default);
}

public class NotificationService : INotificationService
{
private readonly IEmailService \_emailService;
private readonly IOrderRepository \_orderRepository;
private readonly ILogger<NotificationService> \_logger;

    public NotificationService(
        IEmailService emailService,
        IOrderRepository orderRepository,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null) return;

            var email = new EmailMessage(
                to: "customer@example.com", // Would come from customer repository
                subject: $"Order Confirmation - Order #{orderId.Value}",
                body: $@"
                    Your order has been confirmed!
                    Order ID: {orderId.Value}
                    Total: {order.TotalAmount.Amount} {order.TotalAmount.Currency}

                    Thank you for your purchase!");

            await _emailService.SendAsync(email, cancellationToken);
            _logger.LogInformation("Order confirmation sent for order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order confirmation for order {OrderId}", orderId);
        }
    }

    public async Task SendPaymentFailureNotificationAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage(
                to: "customer@example.com",
                subject: $"Payment Failed - Order #{orderId.Value}",
                body: "Your payment could not be processed. Please try again.");

            await _emailService.SendAsync(email, cancellationToken);
            _logger.LogInformation("Payment failure notification sent for order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment failure notification");
        }
    }

    public async Task SendShippingNotificationAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage(
                to: "customer@example.com",
                subject: $"Your Order #{orderId.Value} Has Shipped",
                body: "Your order is on its way!");

            await _emailService.SendAsync(email, cancellationToken);
            _logger.LogInformation("Shipping notification sent for order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send shipping notification");
        }
    }

}

// ============================================================================
// 4. PRESENTATION LAYER - API Controllers
// ============================================================================

namespace OMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
private readonly CreateOrderUseCase \_createOrderUseCase;
private readonly IMediator \_mediator;

    public OrdersController(
        CreateOrderUseCase createOrderUseCase,
        IMediator mediator)
    {
        _createOrderUseCase = createOrderUseCase;
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var useCaseRequest = new CreateOrderRequest(
                request.CustomerId,
                request.Items.Select(i => new OrderItemRequest(i.ProductId, i.Quantity)).ToList(),
                new AddressRequest(request.ShippingAddress.Street, request.ShippingAddress.City,
                    request.ShippingAddress.State, request.ShippingAddress.ZipCode,
                    request.ShippingAddress.Country));

            var result = await _createOrderUseCase.ExecuteAsync(useCaseRequest, cancellationToken);

            return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId },
                new { orderId = result.OrderId, status = result.Status, total = result.TotalAmount });
        }
        catch (InsufficientInventoryException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the order" });
        }
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        // Implementation would fetch order and return OrderResponseDto
        return Ok();
    }

}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
private readonly ProcessPaymentUseCase \_processPaymentUseCase;

    public PaymentsController(ProcessPaymentUseCase processPaymentUseCase)
    {
        _processPaymentUseCase = processPaymentUseCase;
    }

    /// <summary>
    /// Process payment for an order
    /// </summary>
    [HttpPost("process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment(
        [FromBody] PaymentDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var useCaseRequest = new ProcessPaymentRequest(
                request.OrderId,
                request.PaymentMethod,
                request.PaymentToken);

            var result = await _processPaymentUseCase.ExecuteAsync(useCaseRequest, cancellationToken);

            return Ok(new { transactionId = result.TransactionId, status = result.Status });
        }
        catch (OrderNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Payment processing failed" });
        }
    }

}

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
private readonly CheckInventoryUseCase \_checkInventoryUseCase;

    public InventoryController(CheckInventoryUseCase checkInventoryUseCase)
    {
        _checkInventoryUseCase = checkInventoryUseCase;
    }

    /// <summary>
    /// Check inventory availability for a product
    /// </summary>
    [HttpGet("{productId}/availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckAvailability(
        Guid productId,
        [FromQuery] int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _checkInventoryUseCase.ExecuteAsync(
                ProductId.Create(productId),
                quantity,
                cancellationToken);

            return Ok(new
            {
                productId = result.ProductId,
                isAvailable = result.IsAvailable,
                availableQuantity = result.AvailableQuantity
            });
        }
        catch (InventoryNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

}

// ============================================================================
// 5. BASE CLASSES
// ============================================================================

namespace OMS.Domain.Common;

/// <summary>
/// Base class for entities with domain events
/// </summary>
public abstract class Entity
{
private List<DomainEvent> \_domainEvents = new();

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

}

/// <summary>
/// Aggregate root base class
/// </summary>
public abstract class AggregateRoot : Entity
{
}

/// <summary>
/// Value object base class
/// </summary>
public abstract class ValueObject
{
protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

}

// ============================================================================
// 6. DEPENDENCY INJECTION SETUP
// ============================================================================

namespace OMS.Infrastructure;

public static class DependencyInjection
{
public static IServiceCollection AddInfrastructureServices(
this IServiceCollection services,
IConfiguration configuration)
{
// Database
services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // External Services
        services.AddScoped<IPaymentGateway, StripePaymentGateway>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailService, SendGridEmailService>();

        // Use Cases
        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<ProcessPaymentUseCase>();
        services.AddScoped<CheckInventoryUseCase>();

        // Services
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }

}

// In Program.cs:
/\*
var builder = WebApplication.CreateBuilder(args);

builder.Services
.AddInfrastructureServices(builder.Configuration)
.AddApplicationServices()
.AddApiServices();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options => { /_ JWT configuration _/ });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
\*/

// ============================================================================
// 7. ASYNC PROCESSING PATTERNS
// ============================================================================

namespace OMS.Infrastructure.AsyncProcessing;

/// <summary>
/// Background Job Service using Hangfire for long-running operations
/// </summary>
public interface IBackgroundJobService
{
string EnqueueOrderFulfillment(OrderId orderId);
string EnqueueInventoryReconciliation();
string EnqueuePaymentReconciliation();
}

public class HangfireBackgroundJobService : IBackgroundJobService
{
private readonly IBackgroundJobClient \_backgroundJobClient;
private readonly ILogger<HangfireBackgroundJobService> \_logger;

    public HangfireBackgroundJobService(
        IBackgroundJobClient backgroundJobClient,
        ILogger<HangfireBackgroundJobService> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public string EnqueueOrderFulfillment(OrderId orderId)
    {
        _logger.LogInformation("Enqueueing order fulfillment for {OrderId}", orderId.Value);
        return _backgroundJobClient.Enqueue<OrderFulfillmentJob>(
            x => x.ProcessAsync(orderId, JobCancellationToken.Null));
    }

    public string EnqueueInventoryReconciliation()
    {
        _logger.LogInformation("Enqueueing inventory reconciliation");
        return _backgroundJobClient.Enqueue<InventoryReconciliationJob>(
            x => x.ExecuteAsync(JobCancellationToken.Null));
    }

    public string EnqueuePaymentReconciliation()
    {
        _logger.LogInformation("Enqueueing payment reconciliation");
        return _backgroundJobClient.Enqueue<PaymentReconciliationJob>(
            x => x.ExecuteAsync(JobCancellationToken.Null));
    }

}

/// <summary>
/// Order Fulfillment Background Job
/// </summary>
public class OrderFulfillmentJob
{
private readonly IOrderService \_orderService;
private readonly INotificationService \_notificationService;
private readonly ILogger<OrderFulfillmentJob> \_logger;

    public OrderFulfillmentJob(
        IOrderService orderService,
        INotificationService notificationService,
        ILogger<OrderFulfillmentJob> logger)
    {
        _orderService = orderService;
        _notificationService = notificationService;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300 })]
    public async Task ProcessAsync(OrderId orderId, IJobCancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting order fulfillment for {OrderId}", orderId.Value);

            // Simulate fulfillment process (could be shipping label generation, etc.)
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken.ShutdownToken);

            await _orderService.MarkOrderAsShippedAsync(orderId, cancellationToken.ShutdownToken);
            await _notificationService.SendShippingNotificationAsync(orderId);

            _logger.LogInformation("Order fulfillment completed for {OrderId}", orderId.Value);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Order fulfillment cancelled for {OrderId}", orderId.Value);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Order fulfillment failed for {OrderId}", orderId.Value);
            throw;
        }
    }

}

/// <summary>
/// SignalR Hub for Real-time Order Status Updates
/// </summary>
public class OrderHub : Hub
{
private readonly IOrderRepository \_orderRepository;
private readonly ILogger<OrderHub> \_logger;

    public OrderHub(IOrderRepository orderRepository, ILogger<OrderHub> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task SubscribeToOrderAsync(string orderId)
    {
        try
        {
            var parsedOrderId = OrderId.Create(Guid.Parse(orderId));
            var order = await _orderRepository.GetByIdAsync(parsedOrderId, CancellationToken.None);

            if (order == null)
            {
                await Clients.Caller.SendAsync("Error", "Order not found");
                return;
            }

            // Add client to group for this specific order
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");

            // Send current status
            await Clients.Caller.SendAsync("OrderStatusUpdate", new
            {
                OrderId = order.Id.Value,
                Status = order.Status.Value,
                UpdatedAt = DateTime.UtcNow
            });

            _logger.LogInformation(
                "Client {ClientId} subscribed to order {OrderId}",
                Context.ConnectionId, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to order {OrderId}", orderId);
            await Clients.Caller.SendAsync("Error", "Failed to subscribe to order");
        }
    }

    public async Task UnsubscribeFromOrderAsync(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
        _logger.LogInformation(
            "Client {ClientId} unsubscribed from order {OrderId}",
            Context.ConnectionId, orderId);
    }

}

/// <summary>
/// Notification Hub Service for pushing real-time updates
/// </summary>
public interface IOrderStatusHubService
{
Task NotifyOrderStatusChangedAsync(OrderId orderId, string newStatus);
Task NotifyPaymentProcessedAsync(OrderId orderId, PaymentStatus status);
}

public class OrderStatusHubService : IOrderStatusHubService
{
private readonly IHubContext<OrderHub> \_hubContext;
private readonly ILogger<OrderStatusHubService> \_logger;

    public OrderStatusHubService(
        IHubContext<OrderHub> hubContext,
        ILogger<OrderStatusHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyOrderStatusChangedAsync(OrderId orderId, string newStatus)
    {
        try
        {
            await _hubContext.Clients
                .Group($"order-{orderId.Value}")
                .SendAsync("OrderStatusUpdate", new
                {
                    OrderId = orderId.Value,
                    Status = newStatus,
                    UpdatedAt = DateTime.UtcNow,
                    Message = $"Your order status has been updated to {newStatus}"
                });

            _logger.LogInformation(
                "Order status update notification sent for {OrderId}: {Status}",
                orderId.Value, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify order status change for {OrderId}", orderId.Value);
        }
    }

    public async Task NotifyPaymentProcessedAsync(OrderId orderId, PaymentStatus status)
    {
        try
        {
            var message = status == PaymentStatus.Success
                ? "Your payment has been processed successfully"
                : "Payment processing failed. Please try again";

            await _hubContext.Clients
                .Group($"order-{orderId.Value}")
                .SendAsync("PaymentStatusUpdate", new
                {
                    OrderId = orderId.Value,
                    Status = status.Value,
                    UpdatedAt = DateTime.UtcNow,
                    Message = message
                });

            _logger.LogInformation(
                "Payment status update notification sent for {OrderId}: {Status}",
                orderId.Value, status.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify payment status for {OrderId}", orderId.Value);
        }
    }

}

/// <summary>
/// Batch Async Processing - Process multiple items in parallel
/// </summary>
public class BatchInventoryProcessor
{
private readonly IInventoryRepository \_inventoryRepository;
private readonly ILogger<BatchInventoryProcessor> \_logger;

    public BatchInventoryProcessor(
        IInventoryRepository inventoryRepository,
        ILogger<BatchInventoryProcessor> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<Dictionary<ProductId, InventoryAvailability>> CheckBatchAvailabilityAsync(
        List<ProductId> productIds,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking availability for {Count} products", productIds.Count);

        // Process in parallel with throttling
        var results = new Dictionary<ProductId, InventoryAvailability>();
        var semaphore = new SemaphoreSlim(5); // Max 5 concurrent operations

        var tasks = productIds.Select(async productId =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(
                    productId, cancellationToken);

                return (productId, availability: new InventoryAvailability
                {
                    ProductId = productId.Value,
                    Available = inventory?.AvailableQuantity ?? 0,
                    Reserved = inventory?.ReservedQuantity ?? 0
                });
            }
            finally
            {
                semaphore.Release();
            }
        });

        var completedTasks = await Task.WhenAll(tasks);

        foreach (var (productId, availability) in completedTasks)
        {
            results[productId] = availability;
        }

        _logger.LogInformation("Batch availability check completed");
        return results;
    }

}

public class InventoryAvailability
{
public Guid ProductId { get; set; }
public int Available { get; set; }
public int Reserved { get; set; }
}
