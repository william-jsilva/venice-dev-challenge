using MediatR;
using Venice.Orders.Application.Orders.GetOrder;
using Venice.Orders.Domain.Repositories;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Common.Models;

namespace Venice.Orders.Application.Orders.GetOrder;

/// <summary>
/// Handler for processing GetOrderQuery requests
/// </summary>
/// <param name="orderRepository">The order repository</param>
/// <param name="orderItemRepository">The order item repository</param>
/// <param name="cacheService">The cache service</param>
public class GetOrderHandler(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    ICacheService cacheService)
    : IRequestHandler<GetOrderQuery, GetOrderResult?>
{
    /// <summary>
    /// Handles the GetOrderQuery request
    /// </summary>
    /// <param name="request">The GetOrder query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The order details or null if not found</returns>
    public async Task<GetOrderResult?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // Tentar buscar do cache primeiro
        var cacheKey = $"order:{request.Id}";
        var cachedOrder = await cacheService.GetAsync<GetOrderResult>(cacheKey);
        if (cachedOrder != null)
        {
            return cachedOrder;
        }

        // Buscar dados principais do SQL Server
        var order = await orderRepository.GetByIdAsync(request.Id);
        if (order == null)
        {
            return null;
        }

        // Buscar os itens do MongoDB (armazenamento híbrido)
        var orderItems = await orderItemRepository.GetByOrderIdAsync(request.Id);

        // Montar a resposta
        var response = new GetOrderResult
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            Items = orderItems.Select(item => new OrderItemResult
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList()
        };

        // Salvar no cache por 2 minutos
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2));

        return response;
    }
}

