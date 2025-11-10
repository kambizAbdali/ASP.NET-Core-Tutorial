using RedisEcommerceDemo.Models;

namespace RedisEcommerceDemo.Services
{
    public interface IShoppingCartService
    {
        /// <summary>
        /// Retrieves the shopping cart for a specific user from Redis
        /// If cart doesn't exist, creates a new empty cart
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>Shopping cart object</returns>
        Task<ShoppingCart> GetCartAsync(int userId);

        /// <summary>
        /// Adds a product to the user's shopping cart in Redis
        /// If product already exists, updates the quantity
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="item">Cart item to add</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> AddToCartAsync(int userId, CartItem item);

        /// <summary>
        /// Adds a product to cart by product ID with specified quantity
        /// Convenience method that fetches product details first
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="productId">Product ID to add</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> AddToCartAsync(int userId, int productId, int quantity = 1);

        /// <summary>
        /// Removes a specific product from the user's shopping cart
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="productId">Product ID to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> RemoveFromCartAsync(int userId, int productId);

        /// <summary>
        /// Updates the quantity of a specific product in the cart
        /// If quantity is 0 or negative, removes the item
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="productId">Product ID to update</param>
        /// <param name="quantity">New quantity</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> UpdateCartItemQuantityAsync(int userId, int productId, int quantity);

        /// <summary>
        /// Completely clears all items from the user's shopping cart
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> ClearCartAsync(int userId);

        /// <summary>
        /// Calculates and returns the total monetary value of the cart
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>Total amount as decimal</returns>
        Task<decimal> GetCartTotalAsync(int userId);

        /// <summary>
        /// Gets the number of items in the user's shopping cart
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>Number of distinct products in cart</returns>
        Task<int> GetCartItemCountAsync(int userId);

        /// <summary>
        /// Checks if a specific product exists in the user's cart
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="productId">Product ID to check</param>
        /// <returns>True if product exists in cart</returns>
        Task<bool> CartContainsProductAsync(int userId, int productId);

        /// <summary>
        /// Gets detailed information about cart contents and Redis storage
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>Cart information object</returns>
        Task<CartInfo> GetCartInfoAsync(int userId);

        /// <summary>
        /// Merges temporary cart (e.g., from session) with persistent Redis cart
        /// Useful for when users log in
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="temporaryCart">Temporary cart to merge</param>
        /// <returns>True if successful</returns>
        Task<bool> MergeCartsAsync(int userId, ShoppingCart temporaryCart);

        /// <summary>
        /// Clears any corrupted cart data and creates a fresh cart
        /// Useful for fixing serialization issues
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>True if successful</returns>
        Task<bool> ResetCartAsync(int userId);

        /// <summary>
        /// Diagnostic method to check Redis connection and cart health
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>Cart health status</returns>
        Task<CartHealthStatus> GetCartHealthAsync(int userId);
    }
}