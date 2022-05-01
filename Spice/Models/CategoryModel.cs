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

        [Display(Name= "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Display Order")]
        public int Order { get; set; }

    }
}
