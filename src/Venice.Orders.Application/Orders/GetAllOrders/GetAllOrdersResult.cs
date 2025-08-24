using Venice.Orders.Common.Models;

namespace Venice.Orders.Application.Orders.GetAllOrders;

/// <summary>
/// Result of getting all orders
/// </summary>
public class GetAllOrdersResult
{
    /// <summary>
    /// Order ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Order number (formatted)
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Order creation date
    /// </summary>
    public DateTime OrderDate { get; set; }
    
    /// <summary>
    /// Order status
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Total amount of the order
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer email
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// List of order items
    /// </summary>
    public List<OrderItemResult> Items { get; set; } = new();
}
