using System.ComponentModel.DataAnnotations;

namespace Venice.Orders.Application.Dtos;

/// <summary>
/// Request DTO for creating an order
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Customer ID
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// List of order items
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "Order must have at least one item")]
    public List<OrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for order item
/// </summary>
public class OrderItemRequest
{
    /// <summary>
    /// Product name
    /// </summary>
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Quantity
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Unit price
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
}