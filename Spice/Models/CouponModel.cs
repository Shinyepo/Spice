using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Coupon")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }
        public enum ECouponType { Procentowy=0, PLN=1 }

        [Required]
        [Display(Name = "Discount")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Minimum Amount")]
        public double MinimumAmount { get; set; }
        [Display(Name = "Image")]
        public byte[] Image { get; set; }
        [Display(Name = "Active?")]
        public Boolean IsActive { get; set; }
    }
}
