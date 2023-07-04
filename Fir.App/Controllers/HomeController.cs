
using Fir.App.Context;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Fir.App.Controllers
{
    public class HomeController : Controller
    {

        private readonly FirDbContext _context;

		public HomeController(FirDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
        {
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


    }
}