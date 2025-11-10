using StackExchange.Redis;
using System.Text.Json;
using RedisEcommerceDemo.Models;

namespace RedisEcommerceDemo.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRedisService _redisService;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ShoppingCartService> _logger;

        // Redis key pattern for shopping carts
        private const string CartKeyPattern = "cart:user:{0}";

        // Default cart expiration time (24 hours)
        private static readonly TimeSpan DefaultCartExpiration = TimeSpan.FromHours(24);

        public ShoppingCartService(
            IRedisService redisService,
            IProductRepository productRepository,
            ILogger<ShoppingCartService> logger)
        {
            _redisService = redisService;
            _productRepository = productRepository;
            _logger = logger;

            _logger.LogInformation("🛒 ShoppingCartService initialized with Redis storage");
        }

        /// <summary>
        /// Generates Redis key for user's shopping cart
        /// Uses consistent key pattern for easy management
        /// </summary>
        private string GenerateCartKey(int userId)
        {
            return string.Format(CartKeyPattern, userId);
        }

        public async Task<ShoppingCart> GetCartAsync(int userId)
        {
            var cartKey = GenerateCartKey(userId);

            try
            {
                _logger.LogDebug("Retrieving shopping cart for user {UserId} with key {CartKey}", userId, cartKey);

                // Use specialized method for ShoppingCart that handles JSON serialization
                var cart = await _redisService.GetShoppingCartAsync(cartKey);

                if (cart == null)
                {
                    _logger.LogInformation("Creating new shopping cart for user {UserId}", userId);

                    // Create new empty cart if none exists
                    cart = new ShoppingCart
                    {
                        UserId = userId,
                        LastUpdated = DateTime.UtcNow
                    };

                    // Persist the new cart to Redis with JSON serialization
                    bool success = await _redisService.SetShoppingCartAsync(cartKey, cart);

                    if (success)
                    {
                        _logger.LogDebug("New cart created and stored in Redis for user {UserId}", userId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to store new cart in Redis for user {UserId}", userId);
                    }
                }
                else
                {
                    _logger.LogDebug("Cart retrieved from Redis for user {UserId} with {ItemCount} items",
                        userId, cart.Items.Count);
                }

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving shopping cart for user {UserId}", userId);

                // Return empty cart as fallback
                return new ShoppingCart
                {
                    UserId = userId,
                    LastUpdated = DateTime.UtcNow
                };
            }
        }

        public async Task<bool> AddToCartAsync(int userId, CartItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(item.Quantity));

            var cartKey = GenerateCartKey(userId);

            try
            {
                _logger.LogInformation("Adding product {ProductId} to cart for user {UserId} with quantity {Quantity}",
                    item.ProductId, userId, item.Quantity);

                // Get current cart state using specialized method
                var cart = await GetCartAsync(userId);

                // Check if product already exists in cart
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

                if (existingItem != null)
                {
                    // Update quantity if item already exists
                    existingItem.Quantity += item.Quantity;
                    _logger.LogDebug("Updated quantity for existing product {ProductId} in cart. New quantity: {Quantity}",
                        item.ProductId, existingItem.Quantity);
                }
                else
                {
                    // Add new item to cart
                    cart.Items.Add(item);
                    _logger.LogDebug("Added new product {ProductId} to cart with quantity {Quantity}",
                        item.ProductId, item.Quantity);
                }

                // Update cart metadata
                cart.LastUpdated = DateTime.UtcNow;

                // Save updated cart back to Redis using specialized method
                bool success = await _redisService.SetShoppingCartAsync(cartKey, cart);

                if (success)
                {
                    _logger.LogInformation("✅ Successfully added product {ProductId} to cart for user {UserId}. Total items: {TotalItems}",
                        item.ProductId, userId, cart.Items.Count);
                }
                else
                {
                    _logger.LogWarning("❌ Failed to save cart to Redis for user {UserId}", userId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error adding product {ProductId} to cart for user {UserId}",
                    item.ProductId, userId);
                return false;
            }
        }

        public async Task<bool> AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

            try
            {
                _logger.LogDebug("Fetching product details for product ID {ProductId}", productId);

                // Get product details from repository
                var product = await _productRepository.GetProductByIdAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found - cannot add to cart", productId);
                    return false;
                }

                if (product.StockQuantity < quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}. Requested: {Quantity}, Available: {Stock}",
                        productId, quantity, product.StockQuantity);
                    return false;
                }

                // Create cart item from product
                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                };

                // Use main AddToCart method
                return await AddToCartAsync(userId, cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToCart with product ID {ProductId} for user {UserId}",
                    productId, userId);
                return false;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var cartKey = GenerateCartKey(userId);

            try
            {
                _logger.LogInformation("Removing product {ProductId} from cart for user {UserId}", productId, userId);

                var cart = await GetCartAsync(userId);

                // Find and remove the item
                var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (itemToRemove != null)
                {
                    cart.Items.Remove(itemToRemove);
                    cart.LastUpdated = DateTime.UtcNow;

                    // Save updated cart to Redis
                    bool success = await _redisService.SetShoppingCartAsync(cartKey, cart);

                    if (success)
                    {
                        _logger.LogInformation("✅ Successfully removed product {ProductId} from cart for user {UserId}. Remaining items: {RemainingItems}",
                            productId, userId, cart.Items.Count);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to save cart after removal for user {UserId}", userId);
                    }

                    return success;
                }
                else
                {
                    _logger.LogWarning("Product {ProductId} not found in cart for user {UserId}", productId, userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error removing product {ProductId} from cart for user {UserId}",
                    productId, userId);
                return false;
            }
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                // If quantity is 0 or negative, remove the item
                return await RemoveFromCartAsync(userId, productId);
            }

            var cartKey = GenerateCartKey(userId);

            try
            {
                _logger.LogInformation("Updating quantity for product {ProductId} to {Quantity} for user {UserId}",
                    productId, quantity, userId);

                var cart = await GetCartAsync(userId);

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (item != null)
                {
                    // Check stock availability if we had real inventory system
                    // For demo, we'll just update the quantity
                    item.Quantity = quantity;
                    cart.LastUpdated = DateTime.UtcNow;

                    bool success = await _redisService.SetShoppingCartAsync(cartKey, cart);

                    if (success)
                    {
                        _logger.LogInformation("✅ Successfully updated quantity for product {ProductId} to {Quantity} for user {UserId}",
                            productId, quantity, userId);
                    }

                    return success;
                }
                else
                {
                    _logger.LogWarning("Product {ProductId} not found in cart for user {UserId} - cannot update quantity",
                        productId, userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating quantity for product {ProductId} in cart for user {UserId}",
                    productId, userId);
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartKey = GenerateCartKey(userId);

            try
            {
                _logger.LogInformation("Clearing entire cart for user {UserId}", userId);

                // Delete the entire cart from Redis
                bool success = await _redisService.DeleteKeyAsync(cartKey);

                if (success)
                {
                    _logger.LogInformation("✅ Successfully cleared cart for user {UserId}", userId);
                }
                else
                {
                    _logger.LogWarning("Cart might not have existed for user {UserId}", userId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error clearing cart for user {UserId}", userId);
                return false;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            var cart = await GetCartAsync(userId);
            return cart.TotalAmount;
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            var cart = await GetCartAsync(userId);
            return cart.Items.Sum(item => item.Quantity);
        }

        public async Task<bool> CartContainsProductAsync(int userId, int productId)
        {
            var cart = await GetCartAsync(userId);
            return cart.Items.Any(item => item.ProductId == productId);
        }

        public async Task<CartInfo> GetCartInfoAsync(int userId)
        {
            var cartKey = GenerateCartKey(userId);

            try
            {
                var cart = await GetCartAsync(userId);
                var timeToLive = await _redisService.KeyTimeToLiveAsync(cartKey);
                var keyExists = await _redisService.KeyExistsAsync(cartKey);

                return new CartInfo
                {
                    Cart = cart,
                    RedisKey = cartKey,
                    LastAccessed = DateTime.UtcNow,
                    TimeToLive = timeToLive,
                    IsPersisted = keyExists,
                    StorageType = "Redis JSON"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart info for user {UserId}", userId);

                // Return basic info even if Redis fails
                return new CartInfo
                {
                    Cart = await GetCartAsync(userId),
                    RedisKey = cartKey,
                    LastAccessed = DateTime.UtcNow,
                    IsPersisted = false,
                    StorageType = "Temporary (Redis Unavailable)"
                };
            }
        }

        public async Task<bool> MergeCartsAsync(int userId, ShoppingCart temporaryCart)
        {
            if (temporaryCart == null || !temporaryCart.Items.Any())
            {
                _logger.LogDebug("No items to merge for user {UserId}", userId);
                return true;
            }

            try
            {
                _logger.LogInformation("Merging temporary cart with {ItemCount} items into persistent cart for user {UserId}",
                    temporaryCart.Items.Count, userId);

                var persistentCart = await GetCartAsync(userId);
                int mergedItems = 0;

                foreach (var tempItem in temporaryCart.Items)
                {
                    var existingItem = persistentCart.Items.FirstOrDefault(i => i.ProductId == tempItem.ProductId);

                    if (existingItem != null)
                    {
                        // Update quantity for existing items
                        existingItem.Quantity += tempItem.Quantity;
                        mergedItems++;
                    }
                    else
                    {
                        // Add new items
                        persistentCart.Items.Add(tempItem);
                        mergedItems++;
                    }
                }

                // Update cart metadata
                persistentCart.LastUpdated = DateTime.UtcNow;

                // Save merged cart back to Redis
                var cartKey = GenerateCartKey(userId);
                bool success = await _redisService.SetShoppingCartAsync(cartKey, persistentCart);

                if (success)
                {
                    _logger.LogInformation("✅ Successfully merged {MergedItems} items into cart for user {UserId}. Total items: {TotalItems}",
                        mergedItems, userId, persistentCart.Items.Count);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error merging carts for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ResetCartAsync(int userId)
        {
            var cartKey = GenerateCartKey(userId);

            try
            {
                _logger.LogWarning("Resetting cart for user {UserId} due to data corruption", userId);

                // Delete the corrupted cart
                await _redisService.DeleteKeyAsync(cartKey);

                // Create fresh empty cart
                var newCart = new ShoppingCart
                {
                    UserId = userId,
                    LastUpdated = DateTime.UtcNow
                };

                bool success = await _redisService.SetShoppingCartAsync(cartKey, newCart);

                if (success)
                {
                    _logger.LogInformation("✅ Successfully reset cart for user {UserId}", userId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resetting cart for user {UserId}", userId);
                return false;
            }
        }

        public async Task<CartHealthStatus> GetCartHealthAsync(int userId)
        {
            try
            {
                var cartKey = GenerateCartKey(userId);
                var keyExists = await _redisService.KeyExistsAsync(cartKey);
                var cart = await GetCartAsync(userId);

                // Simple ping test
                var pingStart = DateTime.UtcNow;
                var canConnect = await _redisService.KeyExistsAsync("health_test");
                var pingTime = DateTime.UtcNow - pingStart;

                return new CartHealthStatus
                {
                    IsRedisConnected = canConnect,
                    CartExists = keyExists,
                    ItemsCount = cart.Items.Count,
                    TotalValue = cart.TotalAmount,
                    LastUpdated = cart.LastUpdated,
                    IsHealthy = keyExists && cart.Items.All(item => item.Quantity > 0),
                    ErrorMessage = canConnect ? null : "Redis connection failed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cart health for user {UserId}", userId);

                return new CartHealthStatus
                {
                    IsRedisConnected = false,
                    CartExists = false,
                    ItemsCount = 0,
                    TotalValue = 0,
                    LastUpdated = DateTime.UtcNow,
                    IsHealthy = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Helper method to validate cart items (e.g., check stock, prices)
        /// </summary>
        private async Task<bool> ValidateCartItem(CartItem item)
        {
            try
            {
                // In a real application, you might:
                // 1. Verify product exists and is active
                // 2. Check current price matches
                // 3. Validate stock availability
                // 4. Check product restrictions

                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                return product != null && product.StockQuantity >= item.Quantity;
            }
            catch
            {
                // If validation fails, we'll still allow the operation for demo purposes
                return true;
            }
        }
    }
}