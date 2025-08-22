namespace Venice.Orders.Common.Models;

/// <summary>
/// Order item result model shared across the application
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
