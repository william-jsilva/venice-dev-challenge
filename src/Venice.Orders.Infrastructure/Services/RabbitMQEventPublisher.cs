using System.Text.Json;
using RabbitMQ.Client;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.Infrastructure.Services;

public class RabbitMQEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "venice_orders_exchange";

    public RabbitMQEventPublisher(IConnection connection)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        
        // Declarar exchange
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
    }

    public async Task PublishAsync(object @event, CancellationToken cancellationToken = default)
    {
        var eventType = @event.GetType().Name;
        var routingKey = $"order.{eventType.ToLower()}";
        var message = JsonSerializer.Serialize(@event);
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }
}

