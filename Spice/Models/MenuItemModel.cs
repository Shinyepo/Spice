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
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Ostrość")]
        public string Spicyness { get; set; }
        public enum Espicy { NA = 0, Łagodny = 1, Ostry = 2, BardzoOstry = 3 }
        
        [Display(Name = "Zdjęcie")]
        public string Image { get; set; }

        [Display(Name = "Kategoria")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryModel Category { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategoryModel SubCategory { get; set; }

        [Display(Name = "Podkategoria")]
        public int SubCategoryId { get; set; }

        [Display(Name = "Cena")]
        [Range(1, int.MaxValue, ErrorMessage = "Cena musi być większa od ${1}")]
        public double Price { get; set; }

        [Display(Name = "Kolejność wyświetlania")]
        public int Order { get; set; }
    }
}
