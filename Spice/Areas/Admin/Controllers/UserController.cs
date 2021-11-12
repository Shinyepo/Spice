using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _db.ApplicationUser.Where(x=>x.Id != claim.Value).OrderBy(x=>x.Name).ToListAsync());
        }

        public async Task<IActionResult> UnLock(string? id)
        {
            if (id == null) return NotFound();
            var user = await _db.ApplicationUser.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            user.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Lock(string? id)
        {
            if (id == null) return NotFound();
            var user = await _db.ApplicationUser.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            user.LockoutEnd = DateTime.Now.AddYears(100);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
