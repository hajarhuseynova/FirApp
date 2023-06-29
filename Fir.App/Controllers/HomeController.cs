
using Fir.App.Context;
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
            IEnumerable<Category> categories = await _context.Categories.Where(c => !c.IsDeleted)
                .ToListAsync();
            return View(categories);
        }


    }
}