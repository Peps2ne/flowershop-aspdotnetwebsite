using FlowerShopApp.Data;
using FlowerShopApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShopApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDiscountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDiscountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Discounts.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount discount)
        {
            discount.Code = discount.Code.ToUpper().Trim();
            if (ModelState.IsValid)
            {
                _context.Add(discount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();
            discount.IsActive = !discount.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
        // API endpoint for Cart to validate discount codes
        [AllowAnonymous]
        public async Task<IActionResult> Validate(string code)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.Code == code.ToUpper() && d.IsActive);
            if (discount == null)
                return Json(new { valid = false });
            return Json(new { valid = true, percentage = discount.DiscountPercentage, label = $"{discount.Code} - %{discount.DiscountPercentage} İndirim" });
        }
    }
}
