using Fir.App.Context;
using Fir.App.Extentions;
using Fir.App.Helpers;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly FirDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(FirDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products =
                await _context.Products.Where(x => !x.IsDeleted).
                Include(x => x.ProductImages).ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Discounts = await _context.Discounts.Where(x => !x.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Discounts = await _context.Discounts.Where(x => !x.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            if (product.FormFiles.Count ==0 )
            {
                ModelState.AddModelError("","Image's minimum count =1!");
                return View(product); 
            }
            int i = 0;
            product.DiscountId = product.DiscountId == 0 ? null : product.DiscountId;

             foreach (var item in product.FormFiles)
            {
                if (!Helper.isImage(item))
                {
                    ModelState.AddModelError("", "It is not image!");
                    return View(product);
                }
                if (!Helper.isSizeOk(item,1))
                {
                    ModelState.AddModelError("", "Image's size is not suitable!");
                    return View(product);
                }
                ProductImage productImage = new ProductImage
                {
                    CreatedDate = DateTime.Now,
                    Image = item.CreateImage(_environment.WebRootPath, "assets/images/"),
                    Product = product,
                    isMain = i == 0 ? true:false
                };
                i++;
                //product.ProductImages.Add(productImage);
                await _context.ProductImages.AddAsync(productImage);  
            }
             foreach(var item in product.CategoryIds)
            {
                if(! await _context.Categories.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("", "Sen oznu agilli hesab elemee!");
                    return View(product);
                }
                ProductCategory productCategory = new ProductCategory
                {
                    CategoryId = item,
                    Product = product,
                    CreatedDate = DateTime.Now
                };
                await _context.ProductCategories.AddAsync(productCategory);

            }
             foreach (var item in product.TagIds)
            {
                if (!await _context.Tags.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("", "Sen oznu agilli hesab elemee!");
                    return View(product);
                }
                ProductTag productTag = new ProductTag
                {
                    TagId = item,
                    Product = product,
                    CreatedDate = DateTime.Now
                };
                await _context.ProductTags.AddAsync(productTag);
            }
             await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Discounts = await _context.Discounts.Where(x => !x.IsDeleted).ToListAsync();

            Product? product = await _context.Products.
                Where(x => !x.IsDeleted && x.Id == id).
                Include(x => x.ProductImages.Where(x => !x.IsDeleted)).
                Include(x => x.ProductCategories.Where(x => !x.IsDeleted)).
                ThenInclude(x => x.Category).
                Include(x => x.ProductTags.Where(x => !x.IsDeleted)).
                ThenInclude(x => x.Tag).
                FirstOrDefaultAsync();
              
           return View(product);
        }

        public async Task<IActionResult> SetAsMainImage(int id)
        {
            ProductImage productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return Json(new { status = 404 });
            }
            productImage.isMain = true;

            ProductImage? productImage1 = await _context.ProductImages.
             Where(x => x.isMain && x.ProductId==productImage.ProductId).FirstOrDefaultAsync();
            productImage1.isMain = false;
            await _context.SaveChangesAsync();
            return Json(new { status = 200 });
        }
        public async Task<IActionResult> RemoveImage(int id)
        {
            ProductImage? productImage = await _context.ProductImages.
                Where(x => !x.IsDeleted&&x.Id==id).FirstOrDefaultAsync();
            if (productImage == null)
            {
                return Json(new { status = 404,desc="image is not found!" });
            }

            if (productImage.isMain)
            {
                return Json(new { status = 400, desc = "Main image is not removed!" });
            }



            productImage.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Json(new {status=200});  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Discounts = await _context.Discounts.Where(x => !x.IsDeleted).ToListAsync();

            Product? update = await _context.Products.
                AsNoTrackingWithIdentityResolution().
               Where(x => !x.IsDeleted && x.Id == id).
               Include(x => x.ProductImages.Where(x => !x.IsDeleted)).
               Include(x => x.ProductCategories.Where(x => !x.IsDeleted)).
               ThenInclude(x => x.Category).
               Include(x => x.ProductTags.Where(x => !x.IsDeleted)).
               ThenInclude(x => x.Tag).
               FirstOrDefaultAsync();

            product.DiscountId=product.DiscountId==0?null:product.DiscountId;
            if(update == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid){
                return View(update);
            }

            List<ProductTag> RemovableTag= await _context.ProductTags.
                Where(x=>!product.TagIds.Contains(x.TagId))
                .ToListAsync();

            _context.ProductTags.RemoveRange(RemovableTag);

            foreach (var item in product.TagIds)
            {
                if (_context.ProductTags.Where(x => x.ProductId == id &&
                   x.TagId == item).Count() > 0)
                {
                    continue;
                }
                else
                {
                    await _context.ProductTags.AddAsync(new ProductTag
                    {
                        ProductId = id,
                        TagId= item
                    });
                }
               
            }

            List<ProductCategory> RemovableCategory = await _context.ProductCategories.
               Where(x => !product.CategoryIds.Contains(x.CategoryId))
               .ToListAsync();

            _context.ProductCategories.RemoveRange(RemovableCategory);
            foreach (var item in product.CategoryIds)
            {
                if (_context.ProductCategories.Where(x => x.CategoryId == id &&
                   x.CategoryId == item).Count() > 0)
                {
                    continue;
                }
                else
                {
                    await _context.ProductCategories.AddAsync(new ProductCategory
                    {
                        ProductId = id,
                        CategoryId = item
                    });
                }

            }

            if(product.FormFiles!=null && product.FormFiles.Count() > 0)
            {
                foreach (var item in product.FormFiles)
                {
                    if (!Helper.isImage(item))
                    {
                        ModelState.AddModelError("", "It is not image!");
                        return View(update);
                    }
                    if (!Helper.isSizeOk(item, 1))
                    {
                        ModelState.AddModelError("", "Image's size is not suitable!");
                        return View(update);
                    }
                    ProductImage productImage = new ProductImage
                    {
                        CreatedDate = DateTime.Now,
                        Image = item.CreateImage(_environment.WebRootPath, "assets/images/"),
                        Product = product
                       
                    };
                  
                    //product.ProductImages.Add(productImage);
                    await _context.ProductImages.AddAsync(productImage);
                }
            }

             _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

     
       [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Product? product = await
                _context.Products.Where(x=>!x.IsDeleted && x.Id==id).FirstOrDefaultAsync();

            if(product == null)
            {
                return NotFound();
            }
            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
