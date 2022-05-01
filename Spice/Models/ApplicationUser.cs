using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spice.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Street Adress")]
        public string StreetAdress { get; set; }
        [Display(Name = "Street Number")]
        public string StreetNumber { get; set; }
        [Display(Name = "House Number")]
        public string HouseNumber { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        public string PaymentUserId { get; set; }
    }
}
