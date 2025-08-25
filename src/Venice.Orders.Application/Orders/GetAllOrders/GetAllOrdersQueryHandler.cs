using Venice.Orders.Domain.Repositories;
using Venice.Orders.Domain.Entities;
using Venice.Orders.Application.Orders.GetAllOrders;
using Venice.Orders.Common.Models;
using MediatR;

namespace Venice.Orders.Application.Orders.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<GetAllOrdersResult>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderItemRepository _orderItemRepository;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IUserRepository userRepository, IOrderItemRepository orderItemRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<IEnumerable<GetAllOrdersResult>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync();
        var results = new List<GetAllOrdersResult>();

        foreach (var order in orders)
        {
            var user = await _userRepository.GetByIdAsync(order.CustomerId);
            var orderItems = await _orderItemRepository.GetByOrderIdAsync(order.Id);

            var result = new GetAllOrdersResult
            {
                Id = order.Id,
                OrderNumber = $"ORD-{order.Id.ToString("N")[..8].ToUpper()}",
                OrderDate = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CustomerName = user?.Username ?? "Unknown",
                CustomerEmail = user?.Email ?? "Unknown",
                Items = orderItems.Select(item => new OrderItemResult
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice
                }).ToList()
            };

            results.Add(result);
        }

        return results;
    }
}
