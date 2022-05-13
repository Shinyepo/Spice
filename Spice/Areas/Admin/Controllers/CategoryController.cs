using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _db.Category.OrderBy(x => x.Order).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> EditOrder()
        {
            var model = await _db.Category.OrderBy(x => x.Order).ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(int[] locationId)
        {
            var list = await _db.Category.ToListAsync();
            int preference = 1;
            foreach (int id in locationId)
            {
                var Item = list.Find(x=>x.Id == id);
                Item.Order = preference;
                preference += 1;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel Category)
        {
            if (ModelState.IsValid)
            {
                Category.Order = _db.Category.Count() + 1;
                _db.Category.Add(Category);
                await _db.SaveChangesAsync();

                var sub = new SubCategoryModel()
                {
                    CategoryId = Category.Id,
                    Name = "Not Set",
                    Order = _db.SubCategory.Count()
                };
                _db.SubCategory.Add(sub);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(Category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var data = await _db.Category.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel Category)
        {
            if (ModelState.IsValid)
            {
                _db.Update(Category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var data = await _db.Category.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return View();
            }
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var data = await _db.Category.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
    }
}
