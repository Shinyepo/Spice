using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsViewModel CartDetails { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            CartDetails = new OrderDetailsViewModel()
            {
                OrderHeader = new Models.OrderHeaderModel()
            };
            CartDetails.OrderHeader.OrderTotal = 0;

            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                CartDetails.ShoppingCart = await cart.ToListAsync();
                foreach (var item in CartDetails.ShoppingCart)
                {
                    item.MenuItem = await _db.MenuItem.Include(x=>x.Category).FirstOrDefaultAsync(x => x.Id == item.MenuItemId);
                    CartDetails.OrderHeader.OrderTotal += (item.MenuItem.Price + (4 * item.Size)) * item.Count;
                    item.MenuItem.Description = SD.ConvertToRawHtml(item.MenuItem.Description);
                    if (item.MenuItem.Description.Length > 100)
                    {
                        item.MenuItem.Description = item.MenuItem.Description.Substring(0, 99) + "...";
                    }
                }
            }

            CartDetails.OrderHeader.OrderTotalOriginal = CartDetails.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString("ssCouponCode") != null)
            {
                CartDetails.OrderHeader.CouponCode = HttpContext.Session.GetString("ssCouponCode");
                var couponfromdb = await _db.Coupon.Where(x => x.Name.ToLower() == CartDetails.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                CartDetails.OrderHeader.OrderTotal = SD.DiscountedPrice(couponfromdb, CartDetails.OrderHeader.OrderTotalOriginal);
            }
            CartDetails.OrderHeader.OrderTotal = Math.Round(CartDetails.OrderHeader.OrderTotal, 2);

            return View(CartDetails);
        }

        public IActionResult AddCoupon()
        {
            if (CartDetails.OrderHeader.CouponCode == null)
            {
                CartDetails.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString("ssCouponCode", CartDetails.OrderHeader.CouponCode);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString("ssCouponCode", string.Empty);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var fromdb = await _db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            fromdb.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var fromdb = await _db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            if (fromdb.Count == 1)
            {
                _db.ShoppingCart.Remove(fromdb);
                await _db.SaveChangesAsync();
                var cnt = _db.ShoppingCart.Where(x => x.ApplicationUserId == fromdb.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", cnt);
            }
            else
            {
                fromdb.Count -= 1;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var fromdb = await _db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            _db.ShoppingCart.Remove(fromdb);
            await _db.SaveChangesAsync();
            var cnt = _db.ShoppingCart.Where(x => x.ApplicationUserId == fromdb.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32("ssCartCount", cnt);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            CartDetails = new OrderDetailsViewModel()
            {
                OrderHeader = new Models.OrderHeaderModel()
            };
            CartDetails.OrderHeader.OrderTotal = 0;

            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var applicationuser = await _db.ApplicationUser.Where(x => x.Id == claim.Value).FirstOrDefaultAsync();
            var cart = _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                CartDetails.ShoppingCart = cart.ToList();
                foreach (var item in CartDetails.ShoppingCart)
                {
                    item.MenuItem = await _db.MenuItem.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == item.MenuItemId);
                    CartDetails.OrderHeader.OrderTotal += (item.MenuItem.Price + (4 * item.Size)) * item.Count;
                }
            }

            CartDetails.OrderHeader.OrderTotalOriginal = CartDetails.OrderHeader.OrderTotal;
            CartDetails.OrderHeader.PickupName = applicationuser.Name;
            CartDetails.OrderHeader.PhoneNumber = applicationuser.PhoneNumber;
            CartDetails.OrderHeader.PickupTime = DateTime.Now;

            if (HttpContext.Session.GetString("ssCouponCode") != null)
            {
                CartDetails.OrderHeader.CouponCode = HttpContext.Session.GetString("ssCouponCode");
                var couponfromdb = await _db.Coupon.Where(x => x.Name.ToLower() == CartDetails.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                CartDetails.OrderHeader.OrderTotal = SD.DiscountedPrice(couponfromdb, CartDetails.OrderHeader.OrderTotalOriginal);
            }
            CartDetails.OrderHeader.OrderTotal = Math.Round(CartDetails.OrderHeader.OrderTotal, 2);

            return View(CartDetails);
        }

        [HttpPost, ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPOST()
        {

            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            CartDetails.ShoppingCart = await _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value).ToListAsync();


            CartDetails.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            CartDetails.OrderHeader.OrderDate = DateTime.Now;
            CartDetails.OrderHeader.UserId = claim.Value;
            CartDetails.OrderHeader.Status = SD.PaymentStatusPending;
            CartDetails.OrderHeader.PickupTime = Convert.ToDateTime(CartDetails.OrderHeader.PickupDate.ToShortDateString() + " " + CartDetails.OrderHeader.PickupTime.ToShortTimeString());

            List<OrderDetailsModel> orderdetailslist = new List<OrderDetailsModel>();
            _db.OrderHeader.Add(CartDetails.OrderHeader);
            await _db.SaveChangesAsync();

            CartDetails.OrderHeader.OrderTotalOriginal = 0;


            foreach (var item in CartDetails.ShoppingCart)
            {
                item.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(x => x.Id == item.MenuItemId);
                var orderdetails = new OrderDetailsModel()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = CartDetails.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count,
                    Size = item.Size
                };
                CartDetails.OrderHeader.OrderTotalOriginal += (orderdetails.Price + (4 * orderdetails.Size)) * orderdetails.Count;
                _db.OrderDetails.Add(orderdetails);
            }

            if (HttpContext.Session.GetString("ssCouponCode") != null)
            {
                CartDetails.OrderHeader.CouponCode = HttpContext.Session.GetString("ssCouponCode");
                var couponfromdb = await _db.Coupon.Where(x => x.Name.ToLower() == CartDetails.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                CartDetails.OrderHeader.OrderTotal = SD.DiscountedPrice(couponfromdb, CartDetails.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                CartDetails.OrderHeader.OrderTotal = CartDetails.OrderHeader.OrderTotalOriginal;
            }
            CartDetails.OrderHeader.CouponCodeDiscount = CartDetails.OrderHeader.OrderTotalOriginal - CartDetails.OrderHeader.OrderTotal;

            var user = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == claim.Value);

            var options = new SessionCreateOptions
            {
                Customer = user.PaymentUserId,
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmountDecimal = (decimal)CartDetails.OrderHeader.OrderTotal * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = CartDetails.OrderHeader.PickupName,
                            },

                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:44378/Customer/Order/OrderSuccess?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:44378/Customer/Order/OrderCancelled?session_id={CHECKOUT_SESSION_ID}"
            };
            var service = new SessionService();
            Session session = service.Create(options);

            CartDetails.OrderHeader.TransactionId = session.PaymentIntentId;
            _db.ShoppingCart.RemoveRange(CartDetails.ShoppingCart);
            await _db.SaveChangesAsync();
            HttpContext.Session.SetInt32("ssCartCount", 0);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //return RedirectToAction("Confirm", "Order", new { id = CartDetails.OrderHeader.Id });
        }


    }
}
