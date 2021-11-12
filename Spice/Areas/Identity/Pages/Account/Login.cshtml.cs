using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Stripe;

namespace Spice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _db;

        public LoginModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _db.ApplicationUser.Where(x => x.Email == Input.Email).FirstOrDefaultAsync();

                var userClaims = await _userManager.GetClaimsAsync(user);
                var hasClaim = userClaims.FirstOrDefault(x => x.Type == "UserDisplayName");

                if (hasClaim == null)
                {
                    var claim = new Claim("UserDisplayName", user.Name);
                    await _userManager.AddClaimAsync(user, claim);
                }
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    if (user.PaymentUserId == null)
                    {
                        AddressOptions adress = new AddressOptions()
                        {
                            City = user.City,
                            PostalCode = user.PostalCode,
                            Line1 = user.StreetAdress,
                        };
                        var CustomerOptions = new CustomerCreateOptions
                        {
                            Email = user.Email,
                            Phone = user.PhoneNumber,
                            Name = user.Name,
                            Address = adress,
                            Metadata = new Dictionary<string, string>
                        {
                            { "customerId", user.Id },
                        },
                        };
                        var CustomerService = new CustomerService();
                        var customer = CustomerService.Create(CustomerOptions);
                        user.PaymentUserId = customer.Id;
                        await _db.SaveChangesAsync();
                    }
                    
                    List<ShoppingCartModel> cart = await _db.ShoppingCart.Where(x => x.ApplicationUserId == user.Id).ToListAsync();
                    HttpContext.Session.SetInt32("ssCartCount", cart.Count());
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
