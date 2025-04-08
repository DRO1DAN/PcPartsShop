using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcPartsShopDomain.Model;
using PcPartsShopInfrastructure.Models;

namespace PcPartsShopInfrastructure.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly PcPartsShopContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(PcPartsShopContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Add Product to Cart
        public async Task<IActionResult> AddToCart(long id)
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Check if the user already has a cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Status == "Active");

            if (cart == null)
            {
                // If the user doesn't have an active cart, create a new one
                cart = new Cart
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    Status = "Active"
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if the cart already contains the product
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == id);

            if (existingCartItem != null)
            {
                // If the product already exists in the cart, increase the quantity
                existingCartItem.Quantity++;
                _context.CartItems.Update(existingCartItem);
            }
            else
            {
                // If the product doesn't exist in the cart, create a new cart item
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    // Ideally handle the case where product doesn't exist (e.g., return NotFound)
                    return RedirectToAction("Index", "Products");
                }

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = 1,
                    Price = product.Price
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Products");
        }

        // View Cart (Display Cart Items)
        public async Task<IActionResult> ViewCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Status == "Active");

            if (cart == null || !cart.CartItems.Any())
            {
                return View("EmptyCart");
            }

            return View(cart);
        }

        // Remove Item from Cart
        public async Task<IActionResult> RemoveFromCart(long cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        // Update Cart Item Quantity (Optional, if needed)
        [HttpPost]
        public async Task<IActionResult> UpdateCartItemQuantity(long cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product) // Make sure Product data is loaded
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Status == "Active");

            if (cart == null || !cart.CartItems.Any()) // Use .Any() for checking if empty
            {
                TempData["CartMessage"] = "Your cart is empty. Please add items before checking out.";
                return RedirectToAction("ViewCart"); // Redirect if cart is empty
            }

            // *** Create and Populate the ViewModel ***
            var viewModel = new CheckoutViewModel
            {
                CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    // Map data from CartItem and its related Product to CartItemViewModel
                    ProductName = ci.Product?.Name ?? "Product Not Found", // Handle potential null product
                    Price = ci.Price, // Price is on CartItem
                    Quantity = ci.Quantity
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Price * ci.Quantity)
                // ShippingAddress and PhoneNumber will be empty initially, to be filled by the user form
            };

            // Pass the populated ViewModel to the view
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Good! Keep this for security.
        public async Task<IActionResult> SubmitOrder(string shippingAddress, string phoneNumber) // This signature is okay
        {
            // --- Your existing logic ---
            var user = await _userManager.GetUserAsync(User);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product) // Include product needed for potential future logic, though not strictly needed for order creation itself here
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Status == "Active");

            if (cart == null || !cart.CartItems.Any())
            {
                // Maybe add a message indicating the cart was empty upon submission attempt
                TempData["ErrorMessage"] = "Cannot submit order, your cart is empty.";
                return RedirectToAction("ViewCart");
            }

            // Optional: Add validation for the parameters if needed (e.g., check if null/empty)
            if (string.IsNullOrWhiteSpace(shippingAddress) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                // If using parameters, handle validation manually or rely on HTML 'required'
                // If you switched to using CheckoutViewModel as parameter, ModelState.IsValid would be better here.
                TempData["ErrorMessage"] = "Shipping Address and Phone Number are required.";
                // Redirect back to Checkout, potentially repopulating the view model (more complex)
                // For simplicity now, we'll just redirect to Checkout GET which repopulates from the cart
                return RedirectToAction("Checkout");
            }


            // *** Consider wrapping order creation in a transaction for atomicity ***
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = user.Id,
                    ShippingAddress = shippingAddress,
                    PhoneNumber = phoneNumber,
                    TotalPrice = cart.CartItems.Sum(ci => ci.Price * ci.Quantity),
                    OrderDate = DateTime.Now,
                    Status = "Pending"
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Save order to get its ID

                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id, // Use the generated order ID
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price // Use the price stored in the cart item at time of adding
                    };
                    _context.OrderItems.Add(orderItem);
                }
                await _context.SaveChangesAsync(); // Save order items

                // Update cart status *after* order is successfully saved
                cart.Status = "Completed";
                _context.Update(cart);
                await _context.SaveChangesAsync(); // Save cart status update

                // Commit transaction if all steps succeeded
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Your order has been placed successfully!";
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            catch (Exception ex) // Catch potential exceptions during the process
            {
                // Log the exception ex
                await transaction.RollbackAsync(); // Roll back changes if anything failed
                TempData["ErrorMessage"] = "There was an error placing your order. Please try again.";
                // Redirect back to checkout or cart? Checkout might be better.
                return RedirectToAction("Checkout");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(long orderId)
        {
            // Retrieve the order details to display (optional, but good practice)
            var user = await _userManager.GetUserAsync(User); // Get current user
            var order = await _context.Orders
                                    .Include(o => o.OrderItems) // Include items if you want to display them
                                    .ThenInclude(oi => oi.Product) // Include products for item details
                                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id); // IMPORTANT: Ensure the order belongs to the logged-in user

            if (order == null)
            {
                // Handle case where order is not found or doesn't belong to the user
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index", "Home"); // Or redirect to an orders history page
            }

            // The TempData["SuccessMessage"] set in SubmitOrder will be available here
            // You can pass it to the view via ViewBag or just let the view access TempData directly

            return View(order); // Pass the order model to the view
        }

    }
}
