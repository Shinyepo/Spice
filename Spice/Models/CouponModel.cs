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
        [Display(Name = "Kod")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Typ")]
        public string Type { get; set; }
        public enum ECouponType { Procentowy=0, PLN=1 }

        [Required]
        [Display(Name = "Wartość Kuponu")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Minimalna wartość zamówienia")]
        public double MinimumAmount { get; set; }
        [Display(Name = "Zdjęcie")]
        public byte[] Image { get; set; }
        [Display(Name = "Aktywny?")]
        public Boolean IsActive { get; set; }
    }
}
