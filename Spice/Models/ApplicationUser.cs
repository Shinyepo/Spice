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
        [PersonalData]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [PersonalData]
        [Display(Name = "Street Adress")]
        public string StreetAdress { get; set; }
        [PersonalData]
        [Display(Name = "Street Number")]
        public string StreetNumber { get; set; }
        [PersonalData]
        [Display(Name = "House Number")]
        public string HouseNumber { get; set; }
        [PersonalData]
        [Display(Name = "City")]
        public string City { get; set; }
        [PersonalData]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        public string PaymentUserId { get; set; }
    }
}
