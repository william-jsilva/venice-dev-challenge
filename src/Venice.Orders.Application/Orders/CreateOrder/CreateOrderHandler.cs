using MediatR;
using Venice.Orders.Application.Orders.CreateOrder;
using Venice.Orders.Domain.Entities;
using Venice.Orders.Domain.Repositories;
using Venice.Orders.Domain.Events;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Common.Models;

namespace Venice.Orders.Application.Orders.CreateOrder;

/// <summary>
/// Handler for processing CreateOrderCommand requests
/// </summary>
/// <param name="orderRepository">The order repository</param>
/// <param name="orderItemRepository">The order item repository</param>
/// <param name="eventPublisher">The event publisher</param>
/// <param name="cacheService">The cache service</param>
public class CreateOrderHandler(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IEventPublisher eventPublisher,
    ICacheService cacheService) 
    : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    /// <summary>
    /// Handles the CreateOrderCommand request
    /// </summary>
    /// <param name="request">The CreateOrder command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created order details</returns>
    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Criar o pedido (sem os itens para SQL Server)
        var order = new Order
        {
            CustomerId = request.CustomerId
        };

        // Criar os itens do pedido para MongoDB
        var orderItems = request.Items.Select(item => 
            new OrderItem(order.Id, item.ProductName, item.Quantity, item.UnitPrice)).ToList();

        // Calcular total amount
        order.TotalAmount = orderItems.Sum(item => item.TotalPrice);

        // Salvar o pedido no SQL Server (apenas dados principais)
        var createdOrder = await orderRepository.CreateAsync(order);

        // Salvar os itens no MongoDB (armazenamento híbrido)
        await orderItemRepository.CreateManyAsync(orderItems);

        // Publicar evento
        await eventPublisher.PublishAsync(new OrderCreatedEvent(createdOrder), cancellationToken);

        // Invalidar cache
        await cacheService.RemoveAsync($"order:{createdOrder.Id}");

        // Retornar resposta
        return new CreateOrderResult
        {
            Id = createdOrder.Id,
            CustomerId = createdOrder.CustomerId,
            CreatedAt = createdOrder.CreatedAt,
            Status = createdOrder.Status,
            TotalAmount = createdOrder.TotalAmount,
            Items = orderItems.Select(item => new OrderItemResult
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList()
        };
    }
}

