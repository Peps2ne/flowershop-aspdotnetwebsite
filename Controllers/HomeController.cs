using FlowerShopApp.Data;
using FlowerShopApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlowerShopApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var popularProducts = await _context.Products
                .Include(p => p.Category)
                .Take(3)
                .ToListAsync();
            return View(popularProducts);
        }
        
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> ProductDetail(string slug, int? id)
        {
            Product? product = null;
            if (!string.IsNullOrEmpty(slug))
                product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Slug == slug);
            else if (id.HasValue)
                product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var reviews = await _context.Reviews.Include(r => r.User)
                .Where(r => r.ProductId == product.Id && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Reviews = reviews;

            bool isWishlisted = false;
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var wishlist = await _context.Wishlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
                if (wishlist != null && wishlist.Items.Any(i => i.ProductId == product.Id))
                    isWishlisted = true;
            }
            ViewBag.IsWishlisted = isWishlisted;

            return View(product);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now,
                IsApproved = true // Auto-approve for now
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var product = await _context.Products.FindAsync(productId);
            return RedirectToAction("ProductDetail", new { slug = product?.Slug, id = product?.Id });
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ToggleWishlist(int productId)
        {
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Json(new { success = false, message = "Lütfen giriş yapın." });

            var wishlist = await _context.Wishlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId, Items = new List<WishlistItem>() };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
            bool added = false;
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
            }
            else
            {
                _context.WishlistItems.Add(new WishlistItem { WishlistId = wishlist.Id, ProductId = productId });
                added = true;
            }
            await _context.SaveChangesAsync();
            
            return Json(new { success = true, added = added });
        }


        public IActionResult Campaigns() => View();
        public IActionResult About() => View();
        public IActionResult Contact() => View();
    }
}
