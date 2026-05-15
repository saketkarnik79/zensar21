// ============================================================================
// ORDER MANAGEMENT SYSTEM - DTOs, REQUESTS, RESPONSES & CONFIGURATIONS
// ============================================================================

// ============================================================================
// 1. DATA TRANSFER OBJECTS (DTOs) - API Layer Input/Output
// ============================================================================

namespace OMS.Api.Dtos;

#region Order DTOs

/// <summary>
/// DTO for creating a new order
/// </summary>
public class CreateOrderDto
{
public Guid CustomerId { get; set; }
public List<OrderItemDto> Items { get; set; } = new();
public AddressDto ShippingAddress { get; set; } = new();
public PaymentMethodDto PaymentMethod { get; set; } = new();
}

/// <summary>
/// Order item line in request/response
/// </summary>
public class OrderItemDto
{
public Guid ProductId { get; set; }
public int Quantity { get; set; }
public decimal? UnitPrice { get; set; } // Response only
public decimal? TotalPrice { get; set; } // Response only
}

/// <summary>
/// Address DTO for shipping/billing
/// </summary>
public class AddressDto
{
public string Street { get; set; } = string.Empty;
public string City { get; set; } = string.Empty;
public string State { get; set; } = string.Empty;
public string ZipCode { get; set; } = string.Empty;
public string Country { get; set; } = string.Empty;
}

/// <summary>
/// Full order response
/// </summary>
public class OrderResponseDto
{
public Guid OrderId { get; set; }
public Guid CustomerId { get; set; }
public string Status { get; set; } = string.Empty;
public decimal TotalAmount { get; set; }
public string Currency { get; set; } = "USD";
public List<OrderItemDto> Items { get; set; } = new();
public AddressDto ShippingAddress { get; set; } = new();
public PaymentStatusDto? Payment { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Minimal order summary
/// </summary>
public class OrderSummaryDto
{
public Guid OrderId { get; set; }
public string Status { get; set; } = string.Empty;
public decimal TotalAmount { get; set; }
public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request to cancel an order
/// </summary>
public class CancelOrderDto
{
public string Reason { get; set; } = string.Empty;
}

#endregion

#region Payment DTOs

/// <summary>
/// DTO for processing a payment
/// </summary>
public class PaymentDto
{
public Guid OrderId { get; set; }
public decimal Amount { get; set; }
public string Currency { get; set; } = "USD";
public string PaymentMethod { get; set; } = string.Empty; // "card", "paypal", "bank_transfer"
public string PaymentToken { get; set; } = string.Empty; // Token from payment gateway
public string? BillingEmail { get; set; }
}

/// <summary>
/// Payment status response
/// </summary>
public class PaymentStatusDto
{
public Guid PaymentId { get; set; }
public Guid OrderId { get; set; }
public decimal Amount { get; set; }
public string Status { get; set; } = string.Empty; // "Pending", "Processing", "Success", "Failed", "Refunded"
public string PaymentMethod { get; set; } = string.Empty;
public string? TransactionId { get; set; }
public string? FailureReason { get; set; }
public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// Refund request DTO
/// </summary>
public class RefundDto
{
public Guid PaymentId { get; set; }
public decimal? Amount { get; set; } // Partial refund; null = full refund
public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Payment method DTO
/// </summary>
public class PaymentMethodDto
{
public string Type { get; set; } = string.Empty; // "card", "paypal", "bank"
public string Token { get; set; } = string.Empty;
public bool SaveForFuture { get; set; }
}

#endregion

#region Inventory DTOs

/// <summary>
/// Inventory availability response
/// </summary>
public class InventoryAvailabilityDto
{
public Guid ProductId { get; set; }
public bool IsAvailable { get; set; }
public int AvailableQuantity { get; set; }
public int ReservedQuantity { get; set; }
public int TotalQuantity { get; set; }
public DateTime LastRestockDate { get; set; }
public DateTime? EstimatedRestockDate { get; set; }
}

/// <summary>
/// Request to check multiple products' inventory
/// </summary>
public class BatchInventoryCheckDto
{
public List<Guid> ProductIds { get; set; } = new();
}

/// <summary>
/// Response for batch inventory check
/// </summary>
public class BatchInventoryResponseDto
{
public List<InventoryAvailabilityDto> Inventories { get; set; } = new();
public DateTime CheckedAt { get; set; }
}

/// <summary>
/// Inventory update request (Admin only)
/// </summary>
public class UpdateInventoryDto
{
public Guid ProductId { get; set; }
public int QuantityAdjustment { get; set; } // Positive or negative
public string Reason { get; set; } = string.Empty; // "restock", "adjustment", "damage"
}

#endregion

#region Error Response DTOs

/// <summary>
/// Standard error response
/// </summary>
public class ErrorResponseDto
{
public string Message { get; set; } = string.Empty;
public string Code { get; set; } = string.Empty;
public List<ValidationErrorDto>? Errors { get; set; }
public string? CorrelationId { get; set; }
public DateTime Timestamp { get; set; }
}

/// <summary>
/// Validation error details
/// </summary>
public class ValidationErrorDto
{
public string Field { get; set; } = string.Empty;
public string Message { get; set; } = string.Empty;
}

#endregion

// ============================================================================
// 2. USE CASE REQUEST/RESPONSE OBJECTS - Application Layer
// ============================================================================

namespace OMS.Application.UseCases.Orders;

#region Create Order Use Case

public class CreateOrderRequest
{
public Guid CustomerId { get; }
public List<OrderItemRequest> Items { get; }
public AddressRequest ShippingAddress { get; }

    public CreateOrderRequest(Guid customerId, List<OrderItemRequest> items,
        AddressRequest shippingAddress)
    {
        CustomerId = customerId;
        Items = items;
        ShippingAddress = shippingAddress;
    }

}

public class OrderItemRequest
{
public Guid ProductId { get; }
public int Quantity { get; }

    public OrderItemRequest(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

}

public class AddressRequest
{
public string Street { get; }
public string City { get; }
public string State { get; }
public string ZipCode { get; }
public string Country { get; }

    public AddressRequest(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

}

public class CreateOrderResponse
{
public Guid OrderId { get; }
public string Status { get; }
public decimal TotalAmount { get; }

    public CreateOrderResponse(Guid orderId, string status, decimal totalAmount)
    {
        OrderId = orderId;
        Status = status;
        TotalAmount = totalAmount;
    }

}

#endregion

#region Get Order Use Case

public class GetOrderRequest
{
public Guid OrderId { get; }
public GetOrderRequest(Guid orderId) => OrderId = orderId;
}

public class GetOrderResponse
{
public Guid OrderId { get; set; }
public Guid CustomerId { get; set; }
public string Status { get; set; } = string.Empty;
public decimal TotalAmount { get; set; }
public List<OrderItemResponse> Items { get; set; } = new();
public DateTime CreatedAt { get; set; }
}

public class OrderItemResponse
{
public Guid ProductId { get; set; }
public int Quantity { get; set; }
public decimal UnitPrice { get; set; }
public decimal TotalPrice { get; set; }
}

#endregion

#region Cancel Order Use Case

public class CancelOrderRequest
{
public Guid OrderId { get; }
public string Reason { get; }

    public CancelOrderRequest(Guid orderId, string reason)
    {
        OrderId = orderId;
        Reason = reason;
    }

}

public class CancelOrderResponse
{
public Guid OrderId { get; }
public string NewStatus { get; }

    public CancelOrderResponse(Guid orderId, string newStatus)
    {
        OrderId = orderId;
        NewStatus = newStatus;
    }

}

#endregion

namespace OMS.Application.UseCases.Payments;

#region Process Payment Use Case

public class ProcessPaymentRequest
{
public Guid OrderId { get; }
public string PaymentMethod { get; }
public string PaymentToken { get; }

    public ProcessPaymentRequest(Guid orderId, string paymentMethod, string paymentToken)
    {
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        PaymentToken = paymentToken;
    }

}

public class ProcessPaymentResponse
{
public Guid PaymentId { get; }
public string Status { get; }
public string? TransactionId { get; }

    public ProcessPaymentResponse(Guid paymentId, string status, string? transactionId)
    {
        PaymentId = paymentId;
        Status = status;
        TransactionId = transactionId;
    }

}

#endregion

#region Refund Payment Use Case

public class RefundPaymentRequest
{
public Guid PaymentId { get; }
public decimal? Amount { get; }
public string Reason { get; }

    public RefundPaymentRequest(Guid paymentId, decimal? amount, string reason)
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
    }

}

public class RefundPaymentResponse
{
public Guid PaymentId { get; }
public string Status { get; }
public string? RefundTransactionId { get; }

    public RefundPaymentResponse(Guid paymentId, string status, string? refundTransactionId)
    {
        PaymentId = paymentId;
        Status = status;
        RefundTransactionId = refundTransactionId;
    }

}

#endregion

namespace OMS.Application.UseCases.Inventory;

#region Check Inventory Use Case

public class InventoryAvailabilityResponse
{
public Guid ProductId { get; }
public bool IsAvailable { get; }
public int AvailableQuantity { get; }
public DateTime LastRestockDate { get; }

    public InventoryAvailabilityResponse(Guid productId, bool isAvailable,
        int availableQuantity, DateTime lastRestockDate)
    {
        ProductId = productId;
        IsAvailable = isAvailable;
        AvailableQuantity = availableQuantity;
        LastRestockDate = lastRestockDate;
    }

}

#endregion

#region Reserve Inventory Use Case

public class ReserveInventoryRequest
{
public Guid OrderId { get; }
public List<ReserveItemRequest> Items { get; }

    public ReserveInventoryRequest(Guid orderId, List<ReserveItemRequest> items)
    {
        OrderId = orderId;
        Items = items;
    }

}

public class ReserveItemRequest
{
public Guid ProductId { get; }
public int Quantity { get; }

    public ReserveItemRequest(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

}

public class ReservationResult
{
public Guid OrderId { get; }
public bool Success { get; }
public List<Guid> ReservedProductIds { get; }
public string? FailureMessage { get; }

    public ReservationResult(Guid orderId, bool success, List<Guid> reservedProductIds,
        string? failureMessage = null)
    {
        OrderId = orderId;
        Success = success;
        ReservedProductIds = reservedProductIds;
        FailureMessage = failureMessage;
    }

}

#endregion

// ============================================================================
// 3. EXTERNAL SERVICE REQUEST/RESPONSE OBJECTS
// ============================================================================

namespace OMS.Infrastructure.ExternalServices.PaymentGateways;

/// <summary>
/// Request sent to payment gateway
/// </summary>
public class PaymentGatewayRequest
{
public Guid OrderId { get; }
public decimal Amount { get; }
public string PaymentMethod { get; }
public string PaymentToken { get; }

    public PaymentGatewayRequest(Guid orderId, decimal amount, string paymentMethod,
        string paymentToken)
    {
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        PaymentToken = paymentToken;
    }

}

/// <summary>
/// Response from payment gateway
/// </summary>
public class PaymentResult
{
public bool IsSuccessful { get; private set; }
public string TransactionId { get; private set; } = string.Empty;
public string ErrorMessage { get; private set; } = string.Empty;

    private PaymentResult() { }

    public static PaymentResult Success(string transactionId)
        => new() { IsSuccessful = true, TransactionId = transactionId };

    public static PaymentResult Failure(string errorMessage)
        => new() { IsSuccessful = false, ErrorMessage = errorMessage };

}

/// <summary>
/// Refund result from gateway
/// </summary>
public class RefundResult
{
public bool IsSuccessful { get; private set; }
public string RefundId { get; private set; } = string.Empty;
public string ErrorMessage { get; private set; } = string.Empty;

    private RefundResult() { }

    public static RefundResult Success(string refundId)
        => new() { IsSuccessful = true, RefundId = refundId };

    public static RefundResult Failure(string errorMessage)
        => new() { IsSuccessful = false, ErrorMessage = errorMessage };

}

#endregion

// ============================================================================
// 4. CONFIGURATION FILES & APPSETTINGS
// ============================================================================

namespace OMS.Infrastructure.Configuration;

/// <summary>
/// Payment gateway settings from appsettings.json
/// </summary>
public class PaymentGatewaySettings
{
public string Provider { get; set; } = "Stripe"; // Stripe, PayPal
public string ApiKey { get; set; } = string.Empty;
public string SecretKey { get; set; } = string.Empty;
public int TimeoutSeconds { get; set; } = 30;
public int MaxRetries { get; set; } = 3;
public bool TestMode { get; set; }
}

/// <summary>
/// Inventory service settings
/// </summary>
public class InventorySettings
{
public string ServiceBaseUrl { get; set; } = string.Empty;
public string ApiKey { get; set; } = string.Empty;
public int TimeoutSeconds { get; set; } = 15;
public bool UseLocalCache { get; set; } = true;
public int CacheDurationMinutes { get; set; } = 5;
}

/// <summary>
/// Email/Notification settings
/// </summary>
public class NotificationSettings
{
public string EmailProvider { get; set; } = "SendGrid"; // SendGrid, SMTP
public string EmailApiKey { get; set; } = string.Empty;
public string FromEmail { get; set; } = "noreply@ordermanagement.com";
public string SmsProvider { get; set; } = "Twilio";
public string SmsApiKey { get; set; } = string.Empty;
public bool EnableEmailNotifications { get; set; } = true;
public bool EnableSmsNotifications { get; set; } = false;
}

/// <summary>
/// Database settings
/// </summary>
public class DatabaseSettings
{
public string ConnectionString { get; set; } = string.Empty;
public int CommandTimeout { get; set; } = 30;
public int MaxPoolSize { get; set; } = 10;
public bool EnableDetailedErrors { get; set; } = false;
}

/// <summary>
/// Retry policy settings (Polly)
/// </summary>
public class ResilienceSettings
{
public int MaxRetries { get; set; } = 3;
public int InitialDelayMilliseconds { get; set; } = 100;
public double BackoffMultiplier { get; set; } = 2.0;
public int CircuitBreakerThreshold { get; set; } = 5;
public int CircuitBreakerDurationSeconds { get; set; } = 30;
}

/// <summary>
/// Stripe Payment Gateway Configuration
/// </summary>
public class StripeSettings
{
public string PublicKey { get; set; } = string.Empty;
public string SecretKey { get; set; } = string.Empty;
public string WebhookSecret { get; set; } = string.Empty;
public int TimeoutSeconds { get; set; } = 30;
public int RetryMaxAttempts { get; set; } = 3;
public int[] RetryDelaysInSeconds { get; set; } = new[] { 1, 2, 4 };
public bool TestMode { get; set; }
}

/// <summary>
/// PayPal Payment Gateway Configuration
/// </summary>
public class PayPalSettings
{
public string ClientId { get; set; } = string.Empty;
public string ClientSecret { get; set; } = string.Empty;
public string Mode { get; set; } = "Live"; // Live, Sandbox
public string WebhookId { get; set; } = string.Empty;
public int TimeoutSeconds { get; set; } = 30;
public string ApiVersion { get; set; } = "v2";
}

/// <summary>
/// Async Processing Configuration (Hangfire)
/// </summary>
public class AsyncProcessingSettings
{
public string JobStorageType { get; set; } = "SqlServer"; // SqlServer, Redis, InMemory
public string ConnectionString { get; set; } = string.Empty;
public int WorkerCount { get; set; } = 5;
public int MaximumConcurrentRequests { get; set; } = 10;
public int SchedulePollingIntervalSeconds { get; set; } = 15;
public int DefaultJobExpirationDays { get; set; } = 30;
public int FailedJobRetryCount { get; set; } = 3;
}

/// <summary>
/// Redis Cache Configuration
/// </summary>
public class RedisCacheSettings
{
public string ConnectionString { get; set; } = string.Empty;
public string InstanceName { get; set; } = "oms\_";
public int DefaultSlidingExpirationMinutes { get; set; } = 30;
public int AbsoluteExpirationMinutes { get; set; } = 120;
public bool EnableCompression { get; set; } = true;
}

/// <summary>
/// SignalR Real-time Configuration
/// </summary>
public class SignalRSettings
{
public string BackplaneType { get; set; } = "Redis"; // Redis, SqlServer, ServiceBus
public string ConnectionString { get; set; } = string.Empty;
public int MaximumMessageSize { get; set; } = 32 \* 1024; // 32 KB
public int KeepAliveIntervalSeconds { get; set; } = 15;
public int ClientTimeoutIntervalSeconds { get; set; } = 45;
}

#endregion

// ============================================================================
// 8. PAYMENT API REQUEST/RESPONSE DTOs
// ============================================================================

namespace OMS.Api.Dtos.PaymentGateway;

/// <summary>
/// Stripe Webhook Event DTO
/// </summary>
public class StripeWebhookEventDto
{
public string Id { get; set; } = string.Empty;
public string Type { get; set; } = string.Empty;
public StripeEventDataDto Data { get; set; } = new();
public DateTime Created { get; set; }
}

public class StripeEventDataDto
{
public StripePaymentIntentDto Object { get; set; } = new();
public StripePaymentIntentDto PreviousAttributes { get; set; } = new();
}

public class StripePaymentIntentDto
{
public string Id { get; set; } = string.Empty;
public string Status { get; set; } = string.Empty; // succeeded, processing, requires_action, requires_payment_method
public long Amount { get; set; }
public string Currency { get; set; } = string.Empty;
public string? FailureCode { get; set; }
public string? FailureMessage { get; set; }
public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// PayPal Webhook Event DTO
/// </summary>
public class PayPalWebhookEventDto
{
public string Id { get; set; } = string.Empty;
public string EventType { get; set; } = string.Empty; // PAYMENT.CAPTURE.COMPLETED, PAYMENT.CAPTURE.DENIED
public PayPalResourceDto Resource { get; set; } = new();
public DateTime CreateTime { get; set; }
}

public class PayPalResourceDto
{
public string Id { get; set; } = string.Empty;
public string Status { get; set; } = string.Empty;
public PayPalAmountDto Amount { get; set; } = new();
}

public class PayPalAmountDto
{
public string CurrencyCode { get; set; } = string.Empty;
public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Payment Gateway Health Check DTO
/// </summary>
public class PaymentGatewayHealthDto
{
public string GatewayName { get; set; } = string.Empty;
public bool IsHealthy { get; set; }
public string Status { get; set; } = string.Empty; // "Up", "Down", "Degraded"
public decimal? AverageResponseTimeMs { get; set; }
public int SuccessfulRequestsInLastHour { get; set; }
public int FailedRequestsInLastHour { get; set; }
public DateTime CheckedAt { get; set; }
}

#endregion

// ============================================================================
// 9. ASYNC PROCESSING DTOs
// ============================================================================

namespace OMS.Api.Dtos.AsyncProcessing;

/// <summary>
/// Background Job Status DTO
/// </summary>
public class BackgroundJobStatusDto
{
public string JobId { get; set; } = string.Empty;
public string JobType { get; set; } = string.Empty;
public string Status { get; set; } = string.Empty; // Enqueued, Processing, Succeeded, Failed, Deleted
public DateTime CreatedAt { get; set; }
public DateTime? StartedAt { get; set; }
public DateTime? CompletedAt { get; set; }
public string? ErrorMessage { get; set; }
public int RetryCount { get; set; }
public Dictionary<string, object>? JobData { get; set; }
}

/// <summary>
/// Batch Job Processing Request DTO
/// </summary>
public class BatchProcessingRequestDto
{
public string BatchId { get; set; } = string.Empty;
public List<Guid> EntityIds { get; set; } = new();
public string Operation { get; set; } = string.Empty; // "inventory_check", "payment_reconciliation"
public Dictionary<string, object>? Parameters { get; set; }
public int ParallelizationDegree { get; set; } = 5;
}

/// <summary>
/// Batch Job Processing Response DTO
/// </summary>
public class BatchProcessingResponseDto
{
public string BatchId { get; set; } = string.Empty;
public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed
public int TotalCount { get; set; }
public int SuccessCount { get; set; }
public int FailureCount { get; set; }
public List<BatchItemResultDto> Results { get; set; } = new();
public DateTime StartedAt { get; set; }
public DateTime? CompletedAt { get; set; }
}

public class BatchItemResultDto
{
public Guid EntityId { get; set; }
public bool Success { get; set; }
public string? Result { get; set; }
public string? ErrorMessage { get; set; }
}

/// <summary>
/// Real-time Order Status Update via SignalR
/// </summary>
public class OrderStatusUpdateDto
{
public Guid OrderId { get; set; }
public string Status { get; set; } = string.Empty;
public string Message { get; set; } = string.Empty;
public DateTime UpdatedAt { get; set; }
public Dictionary<string, object>? AdditionalData { get; set; }
}

/// <summary>
/// Real-time Payment Status Update via SignalR
/// </summary>
public class PaymentStatusUpdateDto
{
public Guid PaymentId { get; set; }
public Guid OrderId { get; set; }
public string Status { get; set; } = string.Empty;
public string? TransactionId { get; set; }
public string Message { get; set; } = string.Empty;
public DateTime UpdatedAt { get; set; }
}

#endregion

// ============================================================================
// 10. EXTENDED APPSETTINGS.JSON WITH PAYMENT & ASYNC CONFIG
// ============================================================================

/\*
{
"ConnectionStrings": {
"DefaultConnection": "Server=localhost;Database=OrderManagementDb;Trusted_Connection=true;TrustServerCertificate=true;",
"RedisConnection": "localhost:6379,ssl=false",
"HangfireConnection": "Server=localhost;Database=HangfireDb;Trusted_Connection=true;TrustServerCertificate=true;"
},

"PaymentGateway": {
"Provider": "Stripe",
"ApiKey": "pk*live*...",
"SecretKey": "sk*live*...",
"TimeoutSeconds": 30,
"MaxRetries": 3,
"TestMode": false
},

"Stripe": {
"PublicKey": "pk*live*...",
"SecretKey": "sk*live*...",
"WebhookSecret": "whsec\_...",
"TimeoutSeconds": 30,
"RetryMaxAttempts": 3,
"RetryDelaysInSeconds": [1, 2, 4],
"TestMode": false
},

"PayPal": {
"ClientId": "YOUR_PAYPAL_CLIENT_ID",
"ClientSecret": "YOUR_PAYPAL_CLIENT_SECRET",
"Mode": "Live",
"WebhookId": "YOUR_PAYPAL_WEBHOOK_ID",
"TimeoutSeconds": 30,
"ApiVersion": "v2"
},

"AsyncProcessing": {
"JobStorageType": "SqlServer",
"ConnectionString": "Server=localhost;Database=HangfireDb;Trusted_Connection=true;",
"WorkerCount": 5,
"MaximumConcurrentRequests": 10,
"SchedulePollingIntervalSeconds": 15,
"DefaultJobExpirationDays": 30,
"FailedJobRetryCount": 3
},

"Redis": {
"ConnectionString": "localhost:6379,ssl=false",
"InstanceName": "oms\_",
"DefaultSlidingExpirationMinutes": 30,
"AbsoluteExpirationMinutes": 120,
"EnableCompression": true
},

"SignalR": {
"BackplaneType": "Redis",
"ConnectionString": "localhost:6379,ssl=false",
"MaximumMessageSize": 32768,
"KeepAliveIntervalSeconds": 15,
"ClientTimeoutIntervalSeconds": 45
},

"ResilienceSettings": {
"MaxRetries": 3,
"InitialDelayMilliseconds": 100,
"BackoffMultiplier": 2.0,
"CircuitBreakerThreshold": 5,
"CircuitBreakerDurationSeconds": 30
},

"NotificationSettings": {
"EmailProvider": "SendGrid",
"EmailApiKey": "YOUR*SENDGRID_API_KEY",
"FromEmail": "noreply@ordermanagement.com",
"EnableEmailNotifications": true,
"EnableSmsNotifications": false
}
}
*/

// ============================================================================
// 6. FLUENT VALIDATION EXAMPLES
// ============================================================================

namespace OMS.Application.Validators;

using FluentValidation;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
public CreateOrderValidator()
{
RuleFor(x => x.CustomerId)
.NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item")
            .Must(items => items.All(i => i.Quantity > 0))
            .WithMessage("All item quantities must be positive");

        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required")
            .SetValidator(new AddressValidator());
    }

}

public class AddressValidator : AbstractValidator<AddressRequest>
{
public AddressValidator()
{
RuleFor(x => x.Street)
.NotEmpty().WithMessage("Street is required")
.Length(5, 200).WithMessage("Street must be between 5 and 200 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .Length(2, 100).WithMessage("City must be between 2 and 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid zip code format");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required");
    }

}

public class PaymentValidator : AbstractValidator<ProcessPaymentRequest>
{
public PaymentValidator()
{
RuleFor(x => x.OrderId)
.NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required")
            .Must(x => new[] { "card", "paypal", "bank_transfer" }.Contains(x.ToLower()))
            .WithMessage("Invalid payment method");

        RuleFor(x => x.PaymentToken)
            .NotEmpty().WithMessage("Payment token is required");
    }

}

#endregion

// ============================================================================
// 7. MAPPING PROFILES (AutoMapper)
// ============================================================================

namespace OMS.Application.Mapping;

using AutoMapper;
using OMS.Domain.Entities;
using OMS.Api.Dtos;

public class MappingProfile : Profile
{
public MappingProfile()
{
// Order mappings
CreateMap<Order, OrderResponseDto>()
.ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id.Value))
.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value))
.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value))
.ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
.ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount));

        // OrderItem mappings
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.Value))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.Amount));

        // Address mappings
        CreateMap<Address, AddressDto>().ReverseMap();

        // Payment mappings
        CreateMap<Payment, PaymentStatusDto>()
            .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId.Value))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value));

        // Inventory mappings
        CreateMap<Inventory, InventoryAvailabilityDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.Value))
            .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(
                src => src.AvailableQuantity + src.ReservedQuantity));
    }

}

#endregion
