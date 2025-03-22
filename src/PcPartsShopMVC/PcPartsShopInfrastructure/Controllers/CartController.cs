using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcPartsShopDomain.Model;

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

            if (cart == null)
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
    }
}
