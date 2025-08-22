using FluentAssertions;
using Venice.Orders.Domain.Entities;
using Venice.Orders.Domain.Enums;

namespace Venice.Orders.UnitTests;

public class OrderTests
{
    [Fact]
    public void CreateOrder_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var order = new Order();

        // Assert
        order.Id.Should().NotBeEmpty();
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().NotBeNull();
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void CalculateTotalAmount_WithItems_ShouldCalculateCorrectly()
    {
        // Arrange
        var order = new Order();
        order.Items.Add(new OrderItem(order.Id, "Product 1", 2, 10.00m));
        order.Items.Add(new OrderItem(order.Id, "Product 2", 1, 15.00m));

        // Act
        order.CalculateTotalAmount();

        // Assert
        order.TotalAmount.Should().Be(35.00m);
    }

    [Fact]
    public void ConfirmOrder_WhenPending_ShouldChangeStatusToConfirmed()
    {
        // Arrange
        var order = new Order();

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void ConfirmOrder_WhenNotPending_ShouldThrowException()
    {
        // Arrange
        var order = new Order();
        order.Confirm(); // First confirmation

        // Act & Assert
        var action = () => order.Confirm();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Order can only be confirmed when in Pending status");
    }

    [Fact]
    public void CancelOrder_WhenNotDelivered_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var order = new Order();

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void CancelOrder_WhenDelivered_ShouldThrowException()
    {
        // Arrange
        var order = new Order();
        order.Confirm();
        order.Deliver();

        // Act & Assert
        var action = () => order.Cancel();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Delivered orders cannot be cancelled");
    }
}




