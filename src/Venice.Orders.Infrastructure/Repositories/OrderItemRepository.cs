using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Venice.Orders.Domain.Entities;
using Venice.Orders.Domain.Repositories;

namespace Venice.Orders.Infrastructure.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly IMongoCollection<OrderItem> _collection;

    public OrderItemRepository(IMongoDatabase database)
    {
        // Configure Guid serialization for this repository
        ConfigureGuidSerialization();

        _collection = database.GetCollection<OrderItem>("OrderItems");
    }

    private static void ConfigureGuidSerialization()
    {
        // Register a custom Guid serializer that handles the serialization properly
        try
        {
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
        }
        catch (BsonSerializationException ex) when (ex.Message.Contains("already a serializer registered"))
        {
            // Serializer already registered, ignore the exception
        }
    }

    public async Task<OrderItem?> GetByIdAsync(Guid id)
    {
        var filter = Builders<OrderItem>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId)
    {
        var filter = Builders<OrderItem>.Filter.Eq(x => x.OrderId, orderId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<OrderItem> CreateAsync(OrderItem orderItem)
    {
        await _collection.InsertOneAsync(orderItem);
        return orderItem;
    }

    public async Task<OrderItem> UpdateAsync(OrderItem orderItem)
    {
        var filter = Builders<OrderItem>.Filter.Eq(x => x.Id, orderItem.Id);
        var update = Builders<OrderItem>.Update
            .Set(x => x.ProductName, orderItem.ProductName)
            .Set(x => x.Quantity, orderItem.Quantity)
            .Set(x => x.UnitPrice, orderItem.UnitPrice);

        await _collection.UpdateOneAsync(filter, update);
        return orderItem;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<OrderItem>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<OrderItem>> CreateManyAsync(IEnumerable<OrderItem> orderItems)
    {
        if (orderItems.Any())
        {
            await _collection.InsertManyAsync(orderItems);
        }
        return orderItems;
    }

    public async Task DeleteByOrderIdAsync(Guid orderId)
    {
        var filter = Builders<OrderItem>.Filter.Eq(x => x.OrderId, orderId);
        await _collection.DeleteManyAsync(filter);
    }
}
