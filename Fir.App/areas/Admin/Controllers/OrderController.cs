using Fir.App.Context;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;

namespace Fir.App.areas.Admin.Controllersb
{
    [Area("Admin")]

    [Authorize(Roles = "Admin,SuperAdmin")]
    public class OrderController : Controller
    {

        private readonly FirDbContext _context;
        public OrderController(FirDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> orders = await _context.Orders.Where(x => !x.IsDeleted)
                .Include(x=>x.AppUser).
                Include(x => x.orderItems.Where(y => !y.IsDeleted)).
                ThenInclude(x => x.Product).
                ThenInclude(x => x.ProductImages.Where(y => !y.IsDeleted)).
                ToListAsync();
            return View(orders);
        }
        public async Task<IActionResult> Accept(int id)
        {
            Order? order = await _context.Orders.Where(x => !x.isAccepted&& x.Id==id).FirstOrDefaultAsync();
            if(order == null)
            {
                return NotFound();
            }

            order.isAccepted=true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Completed(int id)
        {
            Order? order = await _context.Orders.Where(x => !x.isCompleted && x.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            order.isCompleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Reject(int id)
        {
            Order? order = await _context.Orders.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            order.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }

}
