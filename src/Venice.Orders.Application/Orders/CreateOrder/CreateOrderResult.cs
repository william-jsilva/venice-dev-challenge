using Venice.Orders.Domain.Enums;

namespace Venice.Orders.Application.Orders.CreateOrder;

/// <summary>
/// Result of creating a new order.
/// </summary>
public class CreateOrderResult
{
    /// <summary>
    /// Order ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Customer ID
    /// </summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// Order creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Order status
    /// </summary>
    public OrderStatus Status { get; set; }
    
    /// <summary>
    /// Total amount of the order
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// List of order items
    /// </summary>
    public List<OrderItemResult> Items { get; set; } = new();
}

/// <summary>
/// Order item result
/// </summary>
public class OrderItemResult
{
    /// <summary>
    /// Item ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Total price
    /// </summary>
    public decimal TotalPrice { get; set; }
}

