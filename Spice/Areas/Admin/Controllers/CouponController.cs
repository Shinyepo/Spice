using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
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
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public CouponModel CouponModel { get; set; }

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
            CouponModel = new CouponModel();
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }


        public IActionResult Create()
        {
            return View(CouponModel);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 )
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    CouponModel.Image = p1;                   
                }
                _db.Coupon.Add(CouponModel);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(CouponModel);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var model = await _db.Coupon.FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null) return NotFound();
            if (ModelState.IsValid)
            {
                var fromdb = await _db.Coupon.FindAsync(id);
                if (fromdb == null) return NotFound();
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    fromdb.Image = p1;
                }
                fromdb.Name = CouponModel.Name;
                fromdb.IsActive = CouponModel.IsActive;
                fromdb.Discount = CouponModel.Discount;
                fromdb.MinimumAmount = CouponModel.MinimumAmount;
                fromdb.Type = CouponModel.Type;
               
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(CouponModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var model = await _db.Coupon.FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var model = await _db.Coupon.FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (id == null) return NotFound();
            var model = await _db.Coupon.FindAsync(id);
            if (model == null) return NotFound();
            _db.Coupon.Remove(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
