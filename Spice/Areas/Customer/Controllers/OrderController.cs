using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using System.Text;
using System.Threading.Tasks;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int PageSize = 10;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> OrderSuccess([FromQuery] string session_id)
        {
            if (session_id == null) return NotFound();
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            if (session == null) return NotFound();
            var customerService = new CustomerService();
            var customer = customerService.Get(session.CustomerId);

            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);



            var OrderDetailsViewModel = new OrderDetailsSuccessViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.TransactionId == session.PaymentIntentId && x.UserId == claim.Value)
            };

            OrderDetailsViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
            OrderDetailsViewModel.OrderHeader.Status = SD.StatusSubmited;
            await _db.SaveChangesAsync();
            OrderDetailsViewModel.OrderDetails = await _db.OrderDetails.Where(x => x.OrderId == OrderDetailsViewModel.OrderHeader.Id).Include(x => x.MenuItem).Include(x=>x.MenuItem.Category).ToListAsync();

            return View(OrderDetailsViewModel);
        }

        public async Task<IActionResult> OrderCancelled([FromQuery] string session_id)
        {
            if (session_id == null) return NotFound();
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);
            if (session == null) return NotFound();

            if (session.Status != "open")
            {
                return RedirectToAction("Index", "Home");
            }
            var customerService = new CustomerService();
            var customer = customerService.Get(session.CustomerId);

            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);



            var OrderDetailsViewModel = new OrderDetailsSuccessViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.TransactionId == session.PaymentIntentId && x.UserId == claim.Value)
            };

            OrderDetailsViewModel.OrderHeader.PaymentStatus = SD.StatusCancelled;
            OrderDetailsViewModel.OrderHeader.Status = SD.StatusCancelled;

            await _db.SaveChangesAsync();
            sessionService.Expire(session_id);

            OrderDetailsViewModel.OrderDetails = await _db.OrderDetails.Where(x => x.OrderId == OrderDetailsViewModel.OrderHeader.Id).Include(x=>x.MenuItem).Include(x => x.MenuItem.Category).ToListAsync();

            return View(OrderDetailsViewModel);
        }



        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);


            var orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsSuccessViewModel>()
            };

            List<OrderHeaderModel> OrderHeaderList = await _db.OrderHeader.Include(x => x.ApplicationUser).Where(x => x.UserId == claim.Value).ToListAsync();

            foreach (var item in OrderHeaderList)
            {
                OrderDetailsSuccessViewModel ind = new OrderDetailsSuccessViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(x => x.OrderId == item.Id).Include(x => x.MenuItem).Include(x => x.MenuItem.Category).ToListAsync()
                };
                orderListVM.Orders.Add(ind);
            }

            var count = orderListVM.Orders.Count();
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(x => x.OrderHeader.Id).Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
            orderListVM.PagingInfo = new PagingInfo
            {
                CurrPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                UrlParam = "/Customer/Order/OrderHistory?productPage=:"
            };


            return View(orderListVM);
        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> ManageOrder()
        {
            var OrderVM = new List<OrderDetailsSuccessViewModel>();


            List<OrderHeaderModel> OrderHeaderList = await _db.OrderHeader.Where(x => x.Status == SD.StatusSubmited || x.Status == SD.StatusInProcess).OrderByDescending(x => x.PickupTime).ToListAsync();

            foreach (var item in OrderHeaderList)
            {
                OrderDetailsSuccessViewModel ind = new OrderDetailsSuccessViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(x => x.OrderId == item.Id).Include(x => x.MenuItem).Include(x => x.MenuItem.Category).ToListAsync()
                };
                OrderVM.Add(ind);
            }
            return View(OrderVM.OrderByDescending(x => x.OrderHeader.PickupTime).ToList());
        }

        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            if (OrderId == 0) return NotFound();
            var order = await _db.OrderHeader.FirstOrDefaultAsync(x => x.Id == OrderId);
            if (order == null) return NotFound();
            order.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            if (OrderId == 0) return NotFound();
            var order = await _db.OrderHeader.FirstOrDefaultAsync(x => x.Id == OrderId);
            if (order == null) return NotFound();
            order.Status = SD.StatusReady;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            if (OrderId == 0) return NotFound();
            var order = await _db.OrderHeader.FirstOrDefaultAsync(x => x.Id == OrderId);
            if (order == null) return NotFound();
            order.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        public async Task<IActionResult> GetOrderDetails(int? id)
        {
            if (id == null) NotFound();

            var OrderDetailsViewModel = new OrderDetailsSuccessViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == id),
                OrderDetails = await _db.OrderDetails.Where(x => x.OrderId == id).Include(x => x.MenuItem).Include(x => x.MenuItem.Category).ToListAsync()
            };
            OrderDetailsViewModel.OrderHeader.ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == OrderDetailsViewModel.OrderHeader.UserId);

            return PartialView("_IndividualOrderDetails", OrderDetailsViewModel);
        }

        [Authorize(Roles = SD.FrontDeskUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchEmail = null, string searchPhone = null, string searchName = null)
        {
            //var claims = (ClaimsIdentity)this.User.Identity;
            //var claim = claims.FindFirst(ClaimTypes.NameIdentifier);


            var orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsSuccessViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if (searchName != null) param.Append(searchName);
            param.Append("&searchEmail=");
            if (searchEmail != null) param.Append(searchEmail);
            param.Append("&searchPhone=");
            if (searchPhone != null) param.Append(searchPhone);
            List<OrderHeaderModel> OrderHeaderList = new List<OrderHeaderModel>();
            if (searchName != null || searchEmail != null || searchPhone != null)
            {
                var user = new ApplicationUser();
                if (searchName != null)
                {
                    OrderHeaderList = await _db.OrderHeader.Include(x => x.ApplicationUser).Where(x => x.PickupName.ToLower().Contains(searchName.ToLower())).OrderByDescending(x => x.OrderDate).ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _db.ApplicationUser.Where(x => x.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                        OrderHeaderList = await _db.OrderHeader.Include(x => x.ApplicationUser).Where(x => x.UserId == user.Id).OrderByDescending(x => x.OrderDate).ToListAsync();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderHeaderList = await _db.OrderHeader.Include(x => x.ApplicationUser).Where(x => x.PhoneNumber.Contains(searchPhone)).OrderByDescending(x => x.OrderDate).ToListAsync();
                        }
                    }
                }
            }
            else
            {
                OrderHeaderList = await _db.OrderHeader.Include(x => x.ApplicationUser).Where(x => x.Status == SD.StatusReady).ToListAsync();
            }

            foreach (var item in OrderHeaderList)
            {
                OrderDetailsSuccessViewModel ind = new OrderDetailsSuccessViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(x => x.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(ind);
            }


            var count = orderListVM.Orders.Count();
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(x => x.OrderHeader.Id).Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
            orderListVM.PagingInfo = new PagingInfo
            {
                CurrPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                UrlParam = param.ToString()
            };


            return View(orderListVM);
        }

        [HttpPost, ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int OrderId)
        {
            if (OrderId == 0) return NotFound();
            var fromdb = await _db.OrderHeader.FindAsync(OrderId);
            if (fromdb == null) return NotFound();
            fromdb.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(OrderPickup), "Order"    );
        }
    }
}
