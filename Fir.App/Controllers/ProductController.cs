using Fir.App.Context;
using Fir.App.Services.Implementations;
using Fir.App.Services.Interfaces;
using Fir.App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fir.App.Controllers
{
    public class ProductController : Controller
    {
        private readonly FirDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IBasketService _basketService;

        public ProductController(FirDbContext context, IHttpContextAccessor httpContext, IBasketService basketService)
        {
            _context = context;
            _httpContext = httpContext;
            _basketService = basketService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail(int id)
        {
            ProductViewModel productViewModel = new ProductViewModel
            {
                Product = await _context.Products
                      .Include(x => x.ProductImages.Where(x => !x.IsDeleted))
                       .Include(x => x.ProductTags)
                        .ThenInclude(x => x.Tag)
                        .Include(x => x.ProductCategories)
                        .ThenInclude(x => x.Category)
                      .Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync(),
                Products = await _context.Products
                       .Include(x => x.ProductImages.Where(x => !x.IsDeleted)).Take(4)
                        .Where(x => !x.IsDeleted).ToListAsync()
            };

            return View(productViewModel);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            await _basketService.AddBasket(id);
            return RedirectToAction("index","home");
        }

        public async Task<IActionResult> RemoveBasket(int id)
        {
            await _basketService.Remove(id);
            return RedirectToAction("index", "home");
        }
    }
}
