using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Venice.Orders.Infrastructure.Services;

/// <summary>
/// Service responsible for configuring MongoDB serialization settings
/// </summary>
public static class MongoDbConfigurationService
{
    private static readonly object _configurationLock = new object();
    private static volatile bool _isConfigured = false;

    /// <summary>
    /// Configures MongoDB serialization settings globally
    /// This method is thread-safe and idempotent
    /// </summary>
    public static void ConfigureSerialization()
    {
        // Use double-checked locking pattern for thread safety
        if (_isConfigured) return;

        lock (_configurationLock)
        {
            if (_isConfigured) return;

            try
            {
                ConfigureGuidSerialization();
                ConfigureDateTimeSerialization();
                
                _isConfigured = true;
            }
            catch (Exception ex)
            {
                // Log the exception but don't throw to prevent application startup failure
                System.Diagnostics.Debug.WriteLine($"Failed to configure MongoDB serialization: {ex.Message}");
                _isConfigured = true; // Mark as configured to prevent infinite retries
            }
        }
    }

    /// <summary>
    /// Configures Guid serialization to use string representation
    /// </summary>
    private static void ConfigureGuidSerialization()
    {
        try
        {
            // Register the Guid serializer with string representation
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
            
            // Also register for nullable Guid
            BsonSerializer.RegisterSerializer(typeof(Guid?), 
                new NullableSerializer<Guid>(new GuidSerializer(BsonType.String)));
        }
        catch (BsonSerializationException ex) when (ex.Message.Contains("already a serializer registered"))
        {
            // Serializer already registered, ignore the exception
        }
    }

    /// <summary>
    /// Configures DateTime serialization to use UTC format
    /// </summary>
    private static void ConfigureDateTimeSerialization()
    {
        try
        {
            // Configure DateTime serialization to use UTC
            BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Utc));
            BsonSerializer.RegisterSerializer(typeof(DateTime?), new NullableSerializer<DateTime>(new DateTimeSerializer(DateTimeKind.Utc)));
        }
        catch (BsonSerializationException ex) when (ex.Message.Contains("already a serializer registered"))
        {
            // Serializer already registered, ignore the exception
        }
    }

    /// <summary>
    /// Resets the configuration flag (useful for testing)
    /// </summary>
    internal static void ResetConfiguration()
    {
        lock (_configurationLock)
        {
            _isConfigured = false;
        }
    }
}
