using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Nazwa Kategorii")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kolejność wyświetlania")]
        public int Order { get; set; }

    }
}
