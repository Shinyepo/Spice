using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class OrderHeaderModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "Data złożenia zamówienia")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Suma bez obniżki")]
        public double OrderTotalOriginal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Suma")]
        public double OrderTotal { get; set; }

        [Required]
        [Display(Name ="Czas dostawy")]
        public DateTime PickupTime { get; set; }
        
        [Required]
        [NotMapped]
        [Display(Name = "Dzień dostawy")]
        public DateTime PickupDate { get; set; }

        [Display(Name = "Kupon")]
        public string CouponCode { get; set; }
        public double CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }

        [Display(Name = "Imię odbiorcy")]
        public string PickupName { get; set; }

        [Display(Name = "Numer Telefonu")]
        public string PhoneNumber { get; set; }

        public string TransactionId { get; set; }


    }
}
