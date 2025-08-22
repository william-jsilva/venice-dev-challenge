using System.Text.Json;
using RabbitMQ.Client;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.Infrastructure.Services;

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "venice_orders_exchange";
    private bool _disposed = false;

    public RabbitMQEventPublisher(IConnection connection)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        
        // Declarar exchange
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
    }

    public async Task PublishAsync(object @event, CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMQEventPublisher));
        }

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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _channel?.Close();
            _channel?.Dispose();
            _disposed = true;
        }
    }
}

