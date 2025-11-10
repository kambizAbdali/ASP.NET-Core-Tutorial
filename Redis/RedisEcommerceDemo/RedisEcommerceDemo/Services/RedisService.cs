using StackExchange.Redis;
using System.Text.Json;
using RedisEcommerceDemo.Models;

namespace RedisEcommerceDemo.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _logger = logger;

            _logger.LogInformation("✅ Redis Service initialized successfully");
        }

        // STRING OPERATIONS - Basic key-value storage
        public async Task<bool> SetStringAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                // Serialize object to JSON string for storage
                string serializedValue = JsonSerializer.Serialize(value);
                // Store in Redis with optional expiration
                return await _database.StringSetAsync(key, serializedValue, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting string value for key: {Key}", key);
                return false;
            }
        }

        public async Task<T?> GetStringAsync<T>(string key)
        {
            try
            {
                // Retrieve string value from Redis
                RedisValue redisValue = await _database.StringGetAsync(key);

                // Check if value exists (not null or empty)
                if (redisValue.IsNullOrEmpty)
                {
                    _logger.LogDebug("Key {Key} not found in Redis", key);
                    return default;
                }

                // Deserialize JSON string back to object
                return JsonSerializer.Deserialize<T>(redisValue.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting string value for key: {Key}", key);
                return default;
            }
        }

        public async Task<long> IncrementAsync(string key)
        {
            // Atomic increment operation - thread safe
            return await _database.StringIncrementAsync(key);
        }

        public async Task<long> DecrementAsync(string key)
        {
            // Atomic decrement operation - thread safe
            return await _database.StringDecrementAsync(key);
        }

        // HASH OPERATIONS - For storing objects with multiple fields
        public async Task<bool> SetHashAsync<T>(string key, T obj) where T : class
        {
            try
            {
                // Convert object to dictionary of field-value pairs
                var hashEntries = ObjectToHashEntries(obj);
                // Store all fields in Redis Hash
                await _database.HashSetAsync(key, hashEntries);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting hash for key: {Key}", key);
                return false;
            }
        }

        public async Task<T?> GetHashAsync<T>(string key) where T : class
        {
            try
            {
                // Get all fields from Redis Hash
                HashEntry[] hashEntries = await _database.HashGetAllAsync(key);

                if (hashEntries.Length == 0)
                {
                    return null;
                }

                // Convert hash entries back to object
                return HashEntriesToObject<T>(hashEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hash for key: {Key}", key);
                return null;
            }
        }

        public async Task<bool> DeleteHashAsync(string key)
        {
            // Remove entire hash from Redis
            return await _database.KeyDeleteAsync(key);
        }

        // LIST OPERATIONS - For queue and stack implementations
        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            // Serialize and push to left end of list (for stack behavior)
            string serializedValue = JsonSerializer.Serialize(value);
            return await _database.ListLeftPushAsync(key, serializedValue);
        }

        public async Task<T?> ListLeftPopAsync<T>(string key)
        {
            // Pop from left end of list (LIFO - stack behavior)
            RedisValue value = await _database.ListLeftPopAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            // Serialize and push to right end of list (for queue behavior)
            string serializedValue = JsonSerializer.Serialize(value);
            return await _database.ListRightPushAsync(key, serializedValue);
        }

        public async Task<T?> ListRightPopAsync<T>(string key)
        {
            // Pop from right end of list (FIFO - queue behavior)
            RedisValue value = await _database.ListRightPopAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1)
        {
            // Get range of items from list (all items by default)
            RedisValue[] values = await _database.ListRangeAsync(key, start, stop);
            return values.Select(v => JsonSerializer.Deserialize<T>(v.ToString())).ToList();
        }

        // SET OPERATIONS - For unique collections
        public async Task<bool> SetAddAsync<T>(string key, T value)
        {
            // Add unique member to set (duplicates are ignored)
            string serializedValue = JsonSerializer.Serialize(value);
            return await _database.SetAddAsync(key, serializedValue);
        }

        public async Task<bool> SetRemoveAsync<T>(string key, T value)
        {
            // Remove member from set
            string serializedValue = JsonSerializer.Serialize(value);
            return await _database.SetRemoveAsync(key, serializedValue);
        }

        public async Task<bool> SetContainsAsync<T>(string key, T value)
        {
            // Check if set contains specific member
            string serializedValue = JsonSerializer.Serialize(value);
            return await _database.SetContainsAsync(key, serializedValue);
        }

        public async Task<long> SetLengthAsync(string key)
        {
            // Get number of members in set
            return await _database.SetLengthAsync(key);
        }

        public async Task<HashSet<T>> SetMembersAsync<T>(string key)
        {
            // Get all members of set
            RedisValue[] values = await _database.SetMembersAsync(key);
            return new HashSet<T>(values.Select(v => JsonSerializer.Deserialize<T>(v.ToString())));
        }

        // SORTED SET OPERATIONS - For ranked collections
        public async Task<bool> SortedSetAddAsync(string key, string member, double score)
        {
            // Add member with score to sorted set (for rankings)
            return await _database.SortedSetAddAsync(key, member, score);
        }

        public async Task<List<KeyValuePair<string, double>>> SortedSetRangeByRankAsync(
            string key, long start = 0, long stop = -1, bool descending = false)
        {
            // Get range of members by rank (with scores)
            Order order = descending ? Order.Descending : Order.Ascending;
            SortedSetEntry[] entries = await _database.SortedSetRangeByRankWithScoresAsync(key, start, stop, order);

            return entries.Select(e => new KeyValuePair<string, double>(e.Element, e.Score)).ToList();
        }

        public async Task<double?> SortedSetScoreAsync(string key, string member)
        {
            // Get score of specific member in sorted set
            return await _database.SortedSetScoreAsync(key, member);
        }

        // KEY MANAGEMENT OPERATIONS
        public async Task<bool> KeyExistsAsync(string key)
        {
            // Check if key exists in Redis
            return await _database.KeyExistsAsync(key);
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            // Delete key from Redis
            return await _database.KeyDeleteAsync(key);
        }

        public async Task<TimeSpan?> KeyTimeToLiveAsync(string key)
        {
            // Get remaining time to live for key
            return await _database.KeyTimeToLiveAsync(key);
        }

        // PATTERN-BASED CACHE OPERATION - Most important method
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            try
            {
                // Try to get value from cache first
                T? cachedValue = await GetStringAsync<T>(key);

                if (cachedValue != null && !cachedValue.Equals(default(T)))
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return cachedValue;
                }

                _logger.LogDebug("Cache miss for key: {Key}. Executing factory method.", key);

                // If cache miss, execute factory method to get fresh data
                T freshValue = await factory();

                // Store fresh value in cache for future requests
                if (freshValue != null)
                {
                    await SetStringAsync(key, freshValue, expiry);
                    _logger.LogDebug("Value stored in cache for key: {Key}", key);
                }

                return freshValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrSet for key: {Key}", key);
                // If Redis fails, fall back to factory method
                return await factory();
            }
        }

        // SHOPPING CART SPECIFIC METHODS - NEW & FIXED
        public async Task<bool> SetShoppingCartAsync(string key, ShoppingCart cart)
        {
            try
            {
                // For complex objects with collections, use JSON serialization
                string serializedCart = JsonSerializer.Serialize(cart, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                // Store as string instead of hash for complex objects
                return await _database.StringSetAsync(key, serializedCart, TimeSpan.FromHours(24));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting shopping cart for key: {Key}", key);
                return false;
            }
        }

        public async Task<ShoppingCart?> GetShoppingCartAsync(string key)
        {
            try
            {
                RedisValue redisValue = await _database.StringGetAsync(key);

                if (redisValue.IsNullOrEmpty)
                {
                    _logger.LogDebug("Shopping cart key {Key} not found in Redis", key);
                    return null;
                }

                // Deserialize JSON string back to ShoppingCart object
                var cart = JsonSerializer.Deserialize<ShoppingCart>(redisValue.ToString(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shopping cart for key: {Key}", key);
                return null;
            }
        }

        public async Task<bool> UpdateCartItemAsync(string cartKey, CartItem item)
        {
            try
            {
                // Get current cart
                var cart = await GetShoppingCartAsync(cartKey);

                if (cart == null)
                {
                    cart = new ShoppingCart();
                    // Extract user ID from key if possible (cart:user:1 → 1)
                    if (cartKey.StartsWith("cart:user:"))
                    {
                        if (int.TryParse(cartKey.Split(':')[2], out int userId))
                        {
                            cart.UserId = userId;
                        }
                    }
                }

                // Find existing item or add new one
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = item.Quantity;
                }
                else
                {
                    cart.Items.Add(item);
                }

                cart.LastUpdated = DateTime.UtcNow;

                // Save updated cart
                return await SetShoppingCartAsync(cartKey, cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item for key: {CartKey}", cartKey);
                return false;
            }
        }

        // REDIS CONNECTION INFO - NEW
        public IEnumerable<string> GetEndpoints()
        {
            return _redis.GetEndPoints().Select(ep => ep.ToString());
        }

        public object GetServer(string endpoint)
        {
            return _redis.GetServer(endpoint);
        }

        // PRIVATE HELPER METHODS
        private HashEntry[] ObjectToHashEntries<T>(T obj)
        {
            // Convert object properties to Redis HashEntries
            var properties = typeof(T).GetProperties();
            var hashEntries = new List<HashEntry>();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    // Convert property value to string for Redis storage
                    string stringValue = value is DateTime dateTime ?
                        dateTime.ToString("O") : // ISO 8601 format for dates
                        value.ToString();

                    hashEntries.Add(new HashEntry(prop.Name, stringValue));
                }
            }

            return hashEntries.ToArray();
        }

        private T? HashEntriesToObject<T>(HashEntry[] hashEntries) where T : class
        {
            // Convert Redis HashEntries back to object
            var obj = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var hashEntry = hashEntries.FirstOrDefault(he => he.Name == prop.Name);
                if (hashEntry.Name.HasValue && !hashEntry.Value.IsNullOrEmpty)
                {
                    try
                    {
                        // Convert string value back to appropriate type
                        object convertedValue = Convert.ChangeType(hashEntry.Value.ToString(), prop.PropertyType);
                        prop.SetValue(obj, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to convert value for property {PropertyName}", prop.Name);
                        // Skip this property if conversion fails
                    }
                }
            }

            return obj;
        }
    }
}