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
        [Display(Name = "Imię i Nazwisko")]
        public string Name { get; set; }
        [Display(Name = "Ulica")]
        public string StreetAdress { get; set; }
        [Display(Name = "Numer domu")]
        public string StreetNumber { get; set; }
        [Display(Name = "Numer mieszkania")]
        public string HouseNumber { get; set; }
        [Display(Name = "Miasto")]
        public string City { get; set; }
        [Display(Name = "Kod Pocztowy")]
        public string PostalCode { get; set; }

        public string PaymentUserId { get; set; }
    }
}
