using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class SubCategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Nazwa Podkategorii")]
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name="Kategoria")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryModel Category { get; set; }

        [Display(Name = "Kolejność wyświetlania")]
        public int Order { get; set; }
    }
}
