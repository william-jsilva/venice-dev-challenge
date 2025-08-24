using Venice.Orders.Common.Models;

namespace Venice.Orders.Application.Orders.GetAllOrders;

public record GetAllOrdersResponse(
    Guid Id,
    string OrderNumber,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    string CustomerName,
    string CustomerEmail,
    IEnumerable<OrderItemResult> Items
);
