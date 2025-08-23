using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Venice.Orders.Domain.Entities;
using FluentAssertions;

namespace Venice.Orders.UnitTests;

/// <summary>
/// Tests for MongoDB Guid serialization to ensure the fix is working correctly
/// </summary>
public class MongoDBSerializationTests
{


    [Fact]
    public void GuidSerialization_ShouldWorkWithOrderItem()
    {
        // Arrange
        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            ProductName = "Test Product",
            Quantity = 2,
            UnitPrice = 29.99m
        };

        // Act & Assert - This should not throw an exception
        var action = () =>
        {
            // Test if we can serialize an OrderItem with Guids without the Unspecified error
            var document = orderItem.ToBsonDocument();
            
            // This should not throw the GuidSerializationException
        };

        // This should not throw the GuidSerializationException
        action.Should().NotThrow();
    }
}
