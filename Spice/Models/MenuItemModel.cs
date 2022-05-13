using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class MenuItemModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Spicyness")]
        public string Spicyness { get; set; }
        public enum Espicy { NA = 0, Mild = 1, Spicy = 2, VerySpicy = 3 }
        
        [Display(Name = "Image")]
        public string Image { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryModel Category { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategoryModel SubCategory { get; set; }

        [Display(Name = "Subcategory")]
        public int SubCategoryId { get; set; }

        [Display(Name = "Price")]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than ${1}")]
        public double Price { get; set; }

        [Display(Name = "Display Order")]
        public int Order { get; set; }
    }
}
