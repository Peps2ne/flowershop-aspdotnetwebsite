using FlowerShopApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShopApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddBalance(int id, decimal amount)
        {
            if (amount <= 0) { TempData["Error"] = "Geçersiz tutar."; return RedirectToAction(nameof(Index)); }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Balance += amount;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{user.Username} kullanıcısına {amount:0.00} ₺ eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBalance(int id, decimal amount)
        {
            if (amount <= 0) { TempData["Error"] = "Geçersiz tutar."; return RedirectToAction(nameof(Index)); }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Balance < amount)
            {
                TempData["Error"] = "Kullanıcı bakiyesi yetersiz.";
                return RedirectToAction(nameof(Index));
            }

            user.Balance -= amount;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{user.Username} kullanıcısından {amount:0.00} ₺ çıkarıldı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{user.Username} kullanıcısı silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
