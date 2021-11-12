using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class OrderDetailsModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderHeaderModel OrderHeader { get; set; }


        [Required]
        public int MenuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        public virtual MenuItemModel MenuItem { get; set; }
        [Display(Name = "Ilość")]
        public int Count { get; set; }
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Wielkość")]
        public int Size { get; set; }

        [Required]
        [Display(Name = "Cena")]
        public double Price { get; set; }
    }
}
