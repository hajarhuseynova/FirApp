
using Fir.App.Context;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Fir.App.Controllers
{
    public class HomeController : Controller
    {

        private readonly FirDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(FirDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var jsonBasket = Request.Cookies["basket"];
                if(jsonBasket != null)
                {
                    AppUser appUser=await _userManager.FindByNameAsync(User.Identity.Name);
                    Basket? basket = await _context.Baskets.
                     Where(x => !x.IsDeleted&& x.AppUserId==appUser.Id)
                    .FirstOrDefaultAsync();


                    if (basket == null)
                    {
                         basket = new Basket
                        {
                            CreatedDate = DateTime.Now,
                            AppUser = appUser
                        };
                    await _context.Baskets.AddAsync(basket);
                    }


                    List<BasketViewModel> viewModels= JsonConvert.DeserializeObject<List<BasketViewModel>>(jsonBasket);
                    foreach(var model in viewModels)
                    {
                        BasketItem basketItem = default;
                        if (basket.basketItems != null)
                        {
                            basketItem= basket.basketItems.FirstOrDefault(x=>x.ProductId==model.ProductId);
                        }
                        if (basketItem == null)
                        {

                            basketItem = new BasketItem
                            {
                                Basket = basket,
                                CreatedDate = DateTime.Now,
                                ProductCount = model.Count,
                                ProductId = model.ProductId
                            };
                            await _context.BasketItems.AddAsync(basketItem);

                        }
                        else
                        {
                            basketItem.ProductCount++;
                        }
                    }
                   
                    await _context.SaveChangesAsync();
                    Response.Cookies.Delete("basket");

                }
            }


            //ViewBag.Color = Request.Cookies["color"];
            HomeVM homeVM=new HomeVM();
           homeVM.categories = await _context.Categories.Where(c => !c.IsDeleted)
                .ToListAsync();
            homeVM.blogs = await _context.Blogs.Where(c => !c.IsDeleted)
               .ToListAsync();
            homeVM.employees= await _context.Employees.Where(c => !c.IsDeleted).Include(x=>x.Position)
               .ToListAsync();
            homeVM.products= await _context.Products.
                Where(x=>!x.IsDeleted).Include(x=>x.ProductImages).
                Include(x=>x.Discount).
                Include(x=>x.ProductCategories).ThenInclude(x=>x.Category).
                ToListAsync();
            return View(homeVM);
        }

        //public async Task<IActionResult> ChangeColor(string color)
        //{
        //    var colorcookies= Request.Cookies["color"];
        //    if (string.IsNullOrEmpty(colorcookies))
        //    {
        //        Response.Cookies.Append("color", color);
        //    }
        //    else
        //    {
        //        Response.Cookies.Delete("color");
        //        Response.Cookies.Append("color",color);
        //    }
        //    return RedirectToAction("index", "home");
        //}

    }
}