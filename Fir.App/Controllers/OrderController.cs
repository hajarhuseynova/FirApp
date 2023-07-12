using Fir.App.Context;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.Controllers
{
    [Authorize(Roles ="User")]
    public class OrderController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly FirDbContext _context;


        public OrderController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, FirDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signinManager = signinManager;
            _context = context;
        }
        public async Task<IActionResult> CheckOut()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var baskets = await _context.Baskets.Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).
                Include(x => x.basketItems.Where(y => !y.IsDeleted)).
                ThenInclude(x => x.Product).
                ThenInclude(x => x.ProductImages.Where(y => !y.IsDeleted)).
                  Include(x => x.basketItems.Where(y => !y.IsDeleted)).
                     ThenInclude(x => x.Product).
                ThenInclude(x => x.Discount).FirstOrDefaultAsync();
            if(baskets==null || baskets.basketItems.Count() == 0)
            {
                TempData["empty basket"] = "empty basket";
                return RedirectToAction("index", "home");
            }
            return View(baskets);
        }
        public async Task<IActionResult> CreateOrder()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var baskets = await _context.Baskets.Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).
                Include(x => x.basketItems.Where(y => !y.IsDeleted)).
                ThenInclude(x => x.Product).
                ThenInclude(x => x.ProductImages.Where(y => !y.IsDeleted)).
                  Include(x => x.basketItems.Where(y => !y.IsDeleted)).
                     ThenInclude(x => x.Product).
                ThenInclude(x => x.Discount).FirstOrDefaultAsync();
            if (baskets == null || baskets.basketItems.Count() == 0)
            {
                TempData["empty basket"] = "empty basket";
                return RedirectToAction("index", "home");
            }
            Order order = new Order
            {
                CreatedDate = DateTime.Now,
                AppUserId = appUser.Id
            };

            decimal TotalPrice = 0;
            foreach(var item in baskets.basketItems)
            {
                TotalPrice += (item.Product.Discount == null ? item.Product.Price * item.ProductCount :
                        (item.Product.Price - (item.Product.Price - (decimal)(item.Product.Discount.Percent / 100))) * item.ProductCount);
                OrderItem orderItem = new OrderItem
                {
                    CreatedDate = DateTime.Now,
                    Order=order,
                    ProductId=item.ProductId,
                    ProductCount=item.ProductCount,
                };
                await _context.AddAsync(orderItem);
            }
            order.TotalPrice = TotalPrice;
            await _context.AddAsync(order);
            baskets.IsDeleted = true;
            await _context.SaveChangesAsync();  

            TempData["Order created!"] = "Order Successfully created";
            return RedirectToAction("index", "home");
        }

    }
}
