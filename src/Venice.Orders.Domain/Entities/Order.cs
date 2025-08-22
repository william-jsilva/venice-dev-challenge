using System.ComponentModel.DataAnnotations;
using Venice.Orders.Domain.Enums;

namespace Venice.Orders.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    // NotMapped: Os itens são salvos no MongoDB, não no SQL Server
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public List<OrderItem> Items { get; set; } = new();
    
    public Order()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }
    
    public void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(item => item.Quantity * item.UnitPrice);
    }
    
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Order can only be confirmed when in Pending status");
            
        Status = OrderStatus.Confirmed;
    }
    
    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled");
            
        Status = OrderStatus.Cancelled;
    }
    
    public void Deliver()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Order can only be delivered when in Confirmed status");
            
        Status = OrderStatus.Delivered;
    }
}
