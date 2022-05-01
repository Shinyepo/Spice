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
        [Display(Name = "Count")]
        public int Count { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Size")]
        public int Size { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }
    }
}
