using System.ComponentModel.DataAnnotations;

namespace Venice.Orders.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice => Quantity * UnitPrice;
    
    public OrderItem()
    {
        Id = Guid.NewGuid();
    }
    
    public OrderItem(Guid orderId, string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

