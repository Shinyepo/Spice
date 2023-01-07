using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hosting;

        [BindProperty]
        public MenuItemSubCategoryAndCategoryViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db,
            IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
            MenuItemVM = new MenuItemSubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category,
                MenuItem = new Models.MenuItemModel()
            };
        }

        public async Task<IActionResult> Index()
        {
            var model = await _db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).OrderBy(x=>x.Order).ToListAsync();
            return View(model);
        }

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        public async Task<IActionResult> EditOrder()
        {
            var model = await _db.MenuItem.OrderBy(x => x.Order).ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(int[] locationId)
        {
            var list = await _db.MenuItem.ToListAsync();
            int preference = 1;
            foreach (int id in locationId)
            {
                var Item = list.Find(x => x.Id == id);
                Item.Order = preference;
                preference += 1;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (ModelState.IsValid)
            {
                _db.MenuItem.Add(MenuItemVM.MenuItem);
                await _db.SaveChangesAsync();


                string webRootPath = _hosting.WebRootPath;
                //var files = HttpContext.Request.Form.Files;

                var menuitem = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

                /*if (files.Count > 0)
                {
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    menuitem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
                }
                else
                {
                    var uploads = Path.Combine(webRootPath, @"~\images\" + SD.DefaultFoodImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                    menuitem.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
                }*/
                menuitem.Order = _db.MenuItem.Count() + 1;
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.CategoryList = _db.Category.OrderBy(x => x.Order);
            MenuItemVM.StatusMessage = "Error: Something went wrong while processing your request!";
            return View(MenuItemVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            MenuItemVM.MenuItem = await _db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).SingleOrDefaultAsync(x => x.Id == id);
            if (MenuItemVM.MenuItem == null) return NotFound();
            MenuItemVM.SubCategoryList = await _db.SubCategory.Where(x => x.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null) return NotFound();
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid)
            {
                string webRootPath = _hosting.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var menuitem = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

                /*if (files.Count > 0)
                {
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension = Path.GetExtension(files[0].FileName);

                    var imagePath = Path.Combine(webRootPath, menuitem.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    menuitem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
                }*/

                menuitem.Name = MenuItemVM.MenuItem.Name;
                menuitem.Price = MenuItemVM.MenuItem.Price;
                menuitem.Description = MenuItemVM.MenuItem.Description;
                menuitem.CategoryId = MenuItemVM.MenuItem.CategoryId;
                menuitem.Spicyness = MenuItemVM.MenuItem.Spicyness;
                menuitem.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.SubCategoryList = await _db.SubCategory.Where(x => x.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            return View(MenuItemVM);
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();            
            MenuItemVM.MenuItem = await _db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).SingleOrDefaultAsync(x => x.Id == id);
            if (MenuItemVM.MenuItem == null) return NotFound();
            MenuItemVM.CategoryList = await _db.Category.Where(x => x.Id == MenuItemVM.MenuItem.CategoryId).OrderBy(x => x.Order).ToListAsync();
            MenuItemVM.SubCategoryList = await _db.SubCategory.Where(x => x.Id == MenuItemVM.MenuItem.SubCategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            MenuItemVM.MenuItem = await _db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).SingleOrDefaultAsync(x => x.Id == id);
            if (MenuItemVM.MenuItem == null) return NotFound();
            MenuItemVM.SubCategoryList = await _db.SubCategory.Where(x => x.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (id == null) return NotFound();
            if (ModelState.IsValid)
            {
                var fromdb = await _db.MenuItem.FindAsync(id);

                //string webRootPath = _hosting.WebRootPath;
                //var imagePath = Path.Combine(webRootPath, fromdb.Image.TrimStart('\\'));

                /*if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }*/

                _db.MenuItem.Remove(fromdb);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.SubCategoryList = await _db.SubCategory.Where(x => x.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            return View(MenuItemVM);
        }
    }
}
