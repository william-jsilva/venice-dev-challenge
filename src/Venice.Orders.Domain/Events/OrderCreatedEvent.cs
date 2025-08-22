using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Domain.Events;

public class OrderCreatedEvent
{
    public Order Order { get; }
    
    public OrderCreatedEvent(Order order)
    {
        Order = order;
    }
}

