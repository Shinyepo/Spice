using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _db.SubCategory.Include(x=>x.Category).OrderBy(x=>x.Category.Name).ToListAsync();
            return View(model);
        }

        [ActionName("GetSubCategories")]
        public async Task<IActionResult> GetSubCategories(int id)
        {
            List<SubCategoryModel> SubCategories = new List<SubCategoryModel>();

            SubCategories = await (from SubCategory in _db.SubCategory
                                   where SubCategory.CategoryId == id
                                   select SubCategory).ToListAsync();

            return Json(new SelectList(SubCategories, "Id", "Name"));
        }

        public async Task<IActionResult> Create()
        {
            var model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.OrderBy(x => x.Order).ToListAsync(),
                SubCategory = new Models.SubCategoryModel(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var AlreadyExists = _db.SubCategory.Include(x => x.Category).Where(x => x.Name == model.SubCategory.Name && x.CategoryId == model.SubCategory.CategoryId);
                if (AlreadyExists.Count() > 0)
                {
                    StatusMessage = "Error: Sub Category exists under " + AlreadyExists.First().Category.Name + " category. Please use different name!";
                }
                else
                {

                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }                
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.OrderBy(x => x.Order).ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var data = await _db.SubCategory.SingleOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return NotFound();
            }
            var model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.OrderBy(x => x.Order).ToListAsync(),
                SubCategory = data,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()

            };

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var data = await _db.SubCategory.SingleOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return NotFound();
            }
            var model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.OrderBy(x => x.Order).ToListAsync(),
                SubCategory = data
                //SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()

            };

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var data = await _db.SubCategory.SingleOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return NotFound();
            }
            var model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.OrderBy(x => x.Order).ToListAsync(),
                SubCategory = data
                //SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.SubCategory.FindAsync(id);
            if (category == null)
            {
                return View();
            }
            _db.SubCategory.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var AlreadyExists = _db.SubCategory.Include(x => x.Category).Where(x => x.Name == model.SubCategory.Name && x.CategoryId == model.SubCategory.CategoryId);
                if (AlreadyExists.Count() > 0)
                {
                    StatusMessage = "Error: Sub Category exists under " + AlreadyExists.First().Category.Name + " category. Please use different name!";
                    model.SubCategory.Id = id;
                }
                else
                {
                    var data = await _db.SubCategory.FindAsync(id);
                    data.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.OrderBy(x => x.Order).ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

    }
}
