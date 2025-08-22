using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Domain.Repositories;

public interface IOrderItemRepository
{
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
    Task<OrderItem> CreateAsync(OrderItem orderItem);
    Task<IEnumerable<OrderItem>> CreateManyAsync(IEnumerable<OrderItem> orderItems);
    Task DeleteByOrderIdAsync(Guid orderId);
}

