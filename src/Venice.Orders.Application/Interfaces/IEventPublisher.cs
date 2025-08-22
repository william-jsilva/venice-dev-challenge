namespace Venice.Orders.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(object @event, CancellationToken cancellationToken = default);
}

