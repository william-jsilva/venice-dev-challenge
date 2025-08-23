using Moq;
using FluentAssertions;
using Venice.Orders.Application.Orders.CreateOrder;
using Venice.Orders.Domain.Repositories;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.UnitTests;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CreateOrderHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _cacheServiceMock = new Mock<ICacheService>();
        
        _handler = new CreateOrderHandler(
            _orderRepositoryMock.Object,
            _orderItemRepositoryMock.Object,
            _eventPublisherMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateOrderAndPublishEvent()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = new List<Venice.Orders.Application.Dtos.OrderItemRequest>
            {
                new() { ProductName = "Test Product", Quantity = 2, UnitPrice = 10.00m }
            }
        };

        _orderRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Venice.Orders.Domain.Entities.Order>()))
            .ReturnsAsync((Venice.Orders.Domain.Entities.Order order) => order);

        _orderItemRepositoryMock.Setup(x => x.CreateManyAsync(It.IsAny<IEnumerable<Venice.Orders.Domain.Entities.OrderItem>>()))
            .ReturnsAsync((IEnumerable<Venice.Orders.Domain.Entities.OrderItem> items) => items);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Items.Should().HaveCount(1);
        result.Items.First().ProductName.Should().Be("Test Product");
        result.TotalAmount.Should().Be(20.00m);

        _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Venice.Orders.Domain.Entities.Order>()), Times.Once);
        _orderItemRepositoryMock.Verify(x => x.CreateManyAsync(It.IsAny<IEnumerable<Venice.Orders.Domain.Entities.OrderItem>>()), Times.Once);
        _eventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}



