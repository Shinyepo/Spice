using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(x => x.IsActive == true).ToListAsync()
            };
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cnt = _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", cnt);
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();
            var fromdb = await _db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).Where(x => x.Id == id).FirstOrDefaultAsync();

            var model = new ShoppingCartModel()
            {
                MenuItem = fromdb,
                MenuItemId = fromdb.Id
            };

            return PartialView("_AddPizzaToCartPartial", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCartModel model)
        {
            model.Id = 0;
            if (ModelState.IsValid)
            {
                var claims = (ClaimsIdentity)this.User.Identity;
                var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
                model.ApplicationUserId = claim.Value;

                var cartfromdb = await _db.ShoppingCart.Where(x => x.ApplicationUserId == model.ApplicationUserId && x.MenuItemId == model.MenuItemId && x.Size == model.Size).FirstOrDefaultAsync();

                if (cartfromdb == null)
                {
                    await _db.ShoppingCart.AddAsync(model);
                }
                else
                {
                    cartfromdb.Count += model.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menufromdb = await _db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).Where(x => x.Id == model.MenuItemId).FirstOrDefaultAsync();
                var cartModel = new ShoppingCartModel()
                {
                    MenuItem = menufromdb,
                    MenuItemId = menufromdb.Id
                };
                return View(cartModel);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
