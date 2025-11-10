using Microsoft.AspNetCore.Mvc;
using RedisEcommerceDemo.Models;
using RedisEcommerceDemo.Services;

namespace RedisEcommerceDemo.Controllers
{
    public class CartController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IProductRepository _productRepository;
        private readonly IRedisService _redisService;
        private readonly ILogger<CartController> _logger;

        // Demo user ID - in real application this would come from authentication
        private const int DemoUserId = 1;

        public CartController(
            IShoppingCartService cartService,
            IProductRepository productRepository,
            IRedisService redisService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _productRepository = productRepository;
            _redisService = redisService;
            _logger = logger;

            _logger.LogInformation("🛒 CartController initialized");
        }

        /// <summary>
        /// Main cart page - displays all items in user's shopping cart
        /// Demonstrates Redis hash storage for cart objects
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogDebug("Loading shopping cart page for user {UserId}", DemoUserId);

                // Get cart from Redis using shopping cart service
                var cart = await _cartService.GetCartAsync(DemoUserId);
                var cartInfo = await _cartService.GetCartInfoAsync(DemoUserId);

                // Add Redis information to ViewBag for display
                ViewBag.RedisKey = cartInfo.RedisKey;
                ViewBag.CartInfo = cartInfo;
                ViewBag.IsRedisHealthy = await IsRedisHealthy();

                _logger.LogDebug("Cart loaded successfully with {ItemCount} items for user {UserId}",
                    cart.Items.Count, DemoUserId);

                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart page for user {UserId}", DemoUserId);

                // Return empty cart on error
                var emptyCart = new ShoppingCart { UserId = DemoUserId };
                ViewBag.RedisKey = $"cart:user:{DemoUserId}";
                ViewBag.IsRedisHealthy = false;
                ViewBag.ErrorMessage = "Redis temporarily unavailable - using temporary cart";

                return View(emptyCart);
            }
        }
        // Add this model class to your Models folder or at the bottom of CartController
        public class AddToCartModel
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; } = 1;
            public string ReturnUrl { get; set; } = "";
        }
        /// <summary>
        /// Adds a product to the shopping cart
        /// Uses Redis hash operations to store cart data
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(AddToCartModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid request data";
                return RedirectToAction("Index", "Home");
            }

            if (model.Quantity <= 0)
            {
                TempData["Error"] = "Quantity must be greater than 0";
                return RedirectToLocal(model.ReturnUrl);
            }

            try
            {
                _logger.LogInformation("Adding product {ProductId} to cart for user {UserId} with quantity {Quantity}",
                    model.ProductId, DemoUserId, model.Quantity);

                // Use the service to add item to cart
                bool success = await _cartService.AddToCartAsync(DemoUserId, model.ProductId, model.Quantity);

                if (success)
                {
                    // Get product name for success message
                    var product = await _productRepository.GetProductByIdAsync(model.ProductId);
                    var productName = product?.Name ?? "Product";

                    TempData["Message"] = $"✅ {productName} added to cart successfully!";
                    _logger.LogInformation("Successfully added product {ProductId} to cart", model.ProductId);
                }
                else
                {
                    TempData["Error"] = "Failed to add product to cart. Please try again.";
                    _logger.LogWarning("Failed to add product {ProductId} to cart", model.ProductId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart", model.ProductId);
                TempData["Error"] = "An error occurred while adding the product to cart.";
            }

            return RedirectToLocal(model.ReturnUrl);
        }

        /// <summary>
        /// Updates the quantity of a specific product in the cart
        /// Demonstrates real-time Redis updates
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 0)
            {
                TempData["Error"] = "Quantity cannot be negative";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _logger.LogInformation("Updating quantity for product {ProductId} to {Quantity}", productId, quantity);

                bool success = await _cartService.UpdateCartItemQuantityAsync(DemoUserId, productId, quantity);

                if (success)
                {
                    TempData["Message"] = "Cart updated successfully!";
                    _logger.LogDebug("Quantity updated successfully for product {ProductId}", productId);
                }
                else
                {
                    TempData["Error"] = "Failed to update cart item. The item may not exist in your cart.";
                    _logger.LogWarning("Failed to update quantity for product {ProductId}", productId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity for product {ProductId}", productId);
                TempData["Error"] = "An error occurred while updating the cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Removes a specific product from the shopping cart
        /// Uses Redis hash operations to remove items
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                _logger.LogInformation("Removing product {ProductId} from cart", productId);

                bool success = await _cartService.RemoveFromCartAsync(DemoUserId, productId);

                if (success)
                {
                    TempData["Message"] = "Item removed from cart successfully!";
                    _logger.LogInformation("Successfully removed product {ProductId} from cart", productId);
                }
                else
                {
                    TempData["Error"] = "Failed to remove item from cart. The item may not exist.";
                    _logger.LogWarning("Failed to remove product {ProductId} from cart", productId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from cart", productId);
                TempData["Error"] = "An error occurred while removing the item from cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Completely clears all items from the shopping cart
        /// Demonstrates Redis key deletion for cart management
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                _logger.LogInformation("Clearing entire cart for user {UserId}", DemoUserId);

                bool success = await _cartService.ClearCartAsync(DemoUserId);

                if (success)
                {
                    TempData["Message"] = "Cart cleared successfully!";
                    _logger.LogInformation("Successfully cleared cart for user {UserId}", DemoUserId);
                }
                else
                {
                    TempData["Error"] = "Failed to clear cart. It may already be empty.";
                    _logger.LogWarning("Failed to clear cart for user {UserId}", DemoUserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", DemoUserId);
                TempData["Error"] = "An error occurred while clearing the cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checkout page - demonstrates cart persistence through Redis
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var cart = await _cartService.GetCartAsync(DemoUserId);

                if (!cart.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty. Please add items before checkout.";
                    return RedirectToAction(nameof(Index));
                }

                // Get detailed cart information for display
                var cartInfo = await _cartService.GetCartInfoAsync(DemoUserId);
                ViewBag.CartInfo = cartInfo;
                ViewBag.IsRedisHealthy = await IsRedisHealthy();

                _logger.LogDebug("Loading checkout page for user {UserId} with {ItemCount} items",
                    DemoUserId, cart.Items.Count);

                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading checkout page for user {UserId}", DemoUserId);
                TempData["Error"] = "Unable to load checkout page. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Processes the checkout - in real application would integrate with payment processor
        /// Demonstrates Redis cart usage in complete workflow
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout()
        {
            try
            {
                var cart = await _cartService.GetCartAsync(DemoUserId);

                if (!cart.Items.Any())
                {
                    TempData["Error"] = "Cannot process checkout with empty cart";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation("Processing checkout for user {UserId} with {ItemCount} items totaling ${TotalAmount}",
                    DemoUserId, cart.Items.Count, cart.TotalAmount);

                // In a real application, you would:
                // 1. Process payment with payment gateway
                // 2. Create order in database
                // 3. Update inventory
                // 4. Send confirmation email
                // 5. Clear cart after successful order

                // For demo purposes, we'll simulate successful order processing
                await Task.Delay(1000); // Simulate processing time

                // Store order summary for confirmation page
                var orderSummary = new
                {
                    OrderId = $"REDIS-{DateTime.Now:yyyyMMddHHmmss}",
                    Items = cart.Items,
                    TotalAmount = cart.TotalAmount,
                    ProcessedAt = DateTime.UtcNow
                };

                // Clear cart after successful "order"
                await _cartService.ClearCartAsync(DemoUserId);

                _logger.LogInformation("✅ Checkout processed successfully for user {UserId}. Order ID: {OrderId}",
                    DemoUserId, orderSummary.OrderId);

                TempData["OrderSummary"] = System.Text.Json.JsonSerializer.Serialize(orderSummary);
                return RedirectToAction(nameof(OrderConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing checkout for user {UserId}", DemoUserId);
                TempData["Error"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction(nameof(Checkout));
            }
        }

        /// <summary>
        /// Order confirmation page after successful checkout
        /// </summary>
        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            if (TempData["OrderSummary"] == null)
            {
                TempData["Error"] = "No order found. Please complete checkout first.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var orderSummaryJson = TempData["OrderSummary"] as string;
                var orderSummary = System.Text.Json.JsonSerializer.Deserialize<dynamic>(orderSummaryJson);

                ViewBag.OrderSummary = orderSummary;
                ViewBag.ConfirmationTime = DateTime.UtcNow;

                _logger.LogInformation("Displaying order confirmation for user {UserId}", DemoUserId);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order confirmation for user {UserId}", DemoUserId);
                TempData["Error"] = "Error loading order confirmation.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// API endpoint to get current cart summary (for AJAX updates)
        /// Demonstrates real-time cart data retrieval from Redis
        /// </summary>
        [HttpGet]
        [Route("/api/cart/summary")]
        public async Task<IActionResult> GetCartSummary()
        {
            try
            {
                var cart = await _cartService.GetCartAsync(DemoUserId);
                var itemCount = await _cartService.GetCartItemCountAsync(DemoUserId);
                var totalAmount = await _cartService.GetCartTotalAsync(DemoUserId);

                var summary = new
                {
                    Success = true,
                    ItemCount = itemCount,
                    TotalAmount = totalAmount,
                    TotalItems = cart.Items.Count,
                    LastUpdated = cart.LastUpdated,
                    IsEmpty = !cart.Items.Any()
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart summary via API for user {UserId}", DemoUserId);

                return Ok(new
                {
                    Success = false,
                    ItemCount = 0,
                    TotalAmount = 0,
                    TotalItems = 0,
                    IsEmpty = true,
                    Error = "Redis unavailable"
                });
            }
        }

        /// <summary>
        /// Diagnostic endpoint to check cart health and Redis connection
        /// Useful for monitoring and debugging
        /// </summary>
        [HttpGet]
        [Route("/api/cart/health")]
        public async Task<IActionResult> GetCartHealth()
        {
            try
            {
                var cartService = _cartService as ShoppingCartService;
                if (cartService != null)
                {
                    var healthStatus = await cartService.GetCartHealthAsync(DemoUserId);
                    return Ok(healthStatus);
                }

                return Ok(new
                {
                    IsRedisConnected = await IsRedisHealthy(),
                    Message = "Basic health check available"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cart health check for user {UserId}", DemoUserId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Quick add endpoint for demo products
        /// Useful for testing cart functionality
        /// </summary>
        [HttpPost]
        [Route("/api/cart/quick-add/{productId}")]
        public async Task<IActionResult> QuickAdd(int productId, [FromQuery] int quantity = 1)
        {
            try
            {
                bool success = await _cartService.AddToCartAsync(DemoUserId, productId, quantity);

                if (success)
                {
                    var cart = await _cartService.GetCartAsync(DemoUserId);

                    return Ok(new
                    {
                        Success = true,
                        Message = "Product added to cart",
                        NewItemCount = await _cartService.GetCartItemCountAsync(DemoUserId),
                        TotalAmount = cart.TotalAmount
                    });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Failed to add product to cart" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in quick add for product {ProductId}", productId);
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        #region Helper Methods

        /// <summary>
        /// Safely redirects to return URL or default action
        /// </summary>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index), "Home");
            }
        }

        /// <summary>
        /// Checks if Redis connection is healthy
        /// </summary>
        private async Task<bool> IsRedisHealthy()
        {
            try
            {
                return await _redisService.KeyExistsAsync("health_check");
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}