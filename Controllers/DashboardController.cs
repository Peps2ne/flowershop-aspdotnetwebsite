using FlowerShopApp.Data;
using FlowerShopApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace FlowerShopApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── CART ────────────────────────────────────────────────────────
        public IActionResult Cart(int productId = 0, string name = "", decimal price = 0, string img = "")
        {
            ViewBag.ProductId = productId;
            ViewBag.ProductName = name;
            ViewBag.ProductPrice = price;
            ViewBag.ProductImg = img;
            ViewBag.IsEmpty = string.IsNullOrEmpty(name);
            return View();
        }

        // ─── CHECKOUT ────────────────────────────────────────────────────
        public async Task<IActionResult> Checkout(string cartJson, decimal total)
        {
            if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Products", "Home");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userId != null ? await _context.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == int.Parse(userId)) : null;

            ViewBag.CartJson = cartJson;
            ViewBag.Total = total;
            ViewBag.Balance = user?.Balance ?? 0;
            ViewBag.Addresses = user?.Addresses?.ToList() ?? new List<Address>();
            return View();
        }

        // ─── PROCESS CHECKOUT (Card or Wallet) ───────────────────────────
        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(
            string cartJson, decimal total, int? selectedAddressId, string? newAddressTitle, string? newAddressFullName, string? newAddressPhone, string? newAddressCity, string? newAddressFull,
            string paymentMethod, string cardNumber)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
            int userId = int.TryParse(userIdStr, out int uid) ? uid : 0;

            // Parse cart items from JSON - Using case-insensitive options to match JS lowercase keys
            List<CartItem>? cartItems = null;
            try 
            { 
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson, options); 
            } 
            catch { }
            cartItems ??= new List<CartItem>();

            // Wallet payment: deduct balance
            if (paymentMethod == "wallet" && userId > 0)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.Balance < total)
                {
                    TempData["Error"] = "Bakiye yetersiz.";
                    return RedirectToAction("Checkout", new { cartJson, total });
                }
                user.Balance -= total;
                await _context.SaveChangesAsync();
            }

            string finalAddress = string.Empty;
            if (selectedAddressId.HasValue && selectedAddressId.Value > 0)
            {
                var addr = await _context.Addresses.FindAsync(selectedAddressId.Value);
                if (addr != null && addr.UserId == userId)
                    finalAddress = $"{addr.Title}: {addr.FullName} - {addr.PhoneNumber}\n{addr.City}\n{addr.FullAddress}";
            }
            else if (!string.IsNullOrEmpty(newAddressFull))
            {
                if (userId > 0)
                {
                    var addr = new Address { UserId = userId, Title = string.IsNullOrEmpty(newAddressTitle) ? "Yeni Adres" : newAddressTitle, FullName = newAddressFullName ?? "Bilinmiyor", PhoneNumber = newAddressPhone ?? "-", City = newAddressCity ?? "-", FullAddress = newAddressFull };
                    _context.Addresses.Add(addr);
                    await _context.SaveChangesAsync();
                }
                finalAddress = $"{newAddressFullName} - {newAddressPhone}\n{newAddressCity}\n{newAddressFull}";
            }
            if (string.IsNullOrEmpty(finalAddress)) finalAddress = "Adres Belirtilmedi";

            var order = new Order
            {
                UserId = userIdStr,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                ShippingAddress = finalAddress,
                CardLast4 = paymentMethod == "wallet"
                    ? "BAKİYE"
                    : (cardNumber != null && cardNumber.Length >= 4 ? cardNumber[^4..] : "****")
            };

            foreach (var item in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ProductName = string.IsNullOrEmpty(item.Name) ? "Özel Çiçek Buketi" : item.Name,
                    ProductImage = string.IsNullOrEmpty(item.Img) ? "https://images.unsplash.com/photo-1548094891-c4ba474eb5c1?w=400&q=80" : item.Img,
                    Quantity = item.Qty <= 0 ? 1 : item.Qty,
                    Price = item.Price > 0 ? (item.Price * (item.Qty <= 0 ? 1 : item.Qty)) : total
                });
            }

            if (!order.Items.Any())
            {
                order.Items.Add(new OrderItem { ProductName = "Özel Çiçek Tasarımı", ProductImage = "https://images.unsplash.com/photo-1548094891-c4ba474eb5c1?w=400&q=80", Price = total, Quantity = 1 });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }

        // ─── ORDERS ──────────────────────────────────────────────────────
        public async Task<IActionResult> Orders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "demo_user";
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // ─── BALANCE ─────────────────────────────────────────────────────
        public async Task<IActionResult> Balance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userId != null ? await _context.Users.FindAsync(int.Parse(userId)) : null;
            ViewBag.Balance = user?.Balance ?? 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBalance(decimal amount)
        {
            if (amount <= 0) { TempData["Error"] = "Geçersiz miktar."; return RedirectToAction("Balance"); }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null && int.TryParse(userId, out int uid))
            {
                var user = await _context.Users.FindAsync(uid);
                if (user != null)
                {
                    user.Balance += amount;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"{amount:0.00} ₺ bakiyenize eklendi!";
                }
            }
            return RedirectToAction("Balance");
        }

        // ─── PROFILE / DISCOUNTS / WISHLIST ──────────────────────────────
        public IActionResult Profile() => View();
        public IActionResult Discounts() => View();
        
        public async Task<IActionResult> Wishlists()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var wishlist = await _context.Wishlists
                .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            return View(wishlist);
        }
    }

    // DTO for cart items coming from LocalStorage
    public class CartItem
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("price")]
        public decimal Price { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("img")]
        public string Img { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("qty")]
        public int Qty { get; set; }
    }
}
