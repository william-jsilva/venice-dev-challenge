using System.ComponentModel.DataAnnotations;

namespace Venice.Orders.WebApi.Features.Orders.CreateOrder;

/// <summary>
/// Represents a request to create a new order in the system.
/// </summary>
public record CreateOrderRequest
(
    [Required] Guid CustomerId,
    [Required] List<OrderItemRequest> Items
);

/// <summary>
/// Represents an order item in the request.
/// </summary>
public record OrderItemRequest
(
    [Required] string ProductName,
    [Required] int Quantity,
    [Required] decimal UnitPrice
);

