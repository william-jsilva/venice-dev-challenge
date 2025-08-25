using MediatR;
using Venice.Orders.Application.Dtos;

namespace Venice.Orders.Application.Orders.CreateOrder;

/// <summary>
/// Command for creating a new order.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for creating an order, 
/// including customerId and items list.
/// </remarks>
public class CreateOrderCommand : IRequest<CreateOrderResult>
{
    /// <summary>
    /// Customer ID associated with the order
    /// </summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// List of items in the order
    /// </summary>
    public List<OrderItemRequest> Items { get; set; } = new();
}
