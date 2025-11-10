using RedisEcommerceDemo.Models;

namespace RedisEcommerceDemo.Services
{
    public interface IRedisService
    {
        // String operations
        Task<bool> SetStringAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetStringAsync<T>(string key);
        Task<long> IncrementAsync(string key);
        Task<long> DecrementAsync(string key);

        // Hash operations for objects
        Task<bool> SetHashAsync<T>(string key, T obj) where T : class;
        Task<T?> GetHashAsync<T>(string key) where T : class;
        Task<bool> DeleteHashAsync(string key);

        // List operations
        Task<long> ListLeftPushAsync<T>(string key, T value);
        Task<T?> ListLeftPopAsync<T>(string key);
        Task<long> ListRightPushAsync<T>(string key, T value);
        Task<T?> ListRightPopAsync<T>(string key);
        Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1);

        // Set operations
        Task<bool> SetAddAsync<T>(string key, T value);
        Task<bool> SetRemoveAsync<T>(string key, T value);
        Task<bool> SetContainsAsync<T>(string key, T value);
        Task<long> SetLengthAsync(string key);
        Task<HashSet<T>> SetMembersAsync<T>(string key);

        // Sorted Set operations
        Task<bool> SortedSetAddAsync(string key, string member, double score);
        Task<List<KeyValuePair<string, double>>> SortedSetRangeByRankAsync(string key, long start = 0, long stop = -1, bool descending = false);
        Task<double?> SortedSetScoreAsync(string key, string member);

        // Key management
        Task<bool> KeyExistsAsync(string key);
        Task<bool> DeleteKeyAsync(string key);
        Task<TimeSpan?> KeyTimeToLiveAsync(string key);

        // Utility methods
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);

        // Shopping Cart specific methods - NEW
        Task<bool> SetShoppingCartAsync(string key, ShoppingCart cart);
        Task<ShoppingCart?> GetShoppingCartAsync(string key);
        Task<bool> UpdateCartItemAsync(string cartKey, CartItem item);

        // Redis connection info - NEW
        IEnumerable<string> GetEndpoints();
        object GetServer(string endpoint);
    }

    /// <summary>
    /// Extended information about the shopping cart including Redis metadata
    /// </summary>
    public class CartInfo
    {
        public ShoppingCart Cart { get; set; } = new ShoppingCart();
        public string RedisKey { get; set; } = string.Empty;
        public DateTime LastAccessed { get; set; }
        public TimeSpan? TimeToLive { get; set; }
        public int TotalItems => Cart.Items.Sum(item => item.Quantity);
        public decimal TotalValue => Cart.TotalAmount;
        public bool IsPersisted { get; set; }
        public string StorageType { get; set; } = "Redis JSON";
    }

    /// <summary>
    /// Health status information for shopping cart
    /// </summary>
    public class CartHealthStatus
    {
        public bool IsRedisConnected { get; set; }
        public bool CartExists { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsHealthy { get; set; }
        public string? ErrorMessage { get; set; }
    }
}