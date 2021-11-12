using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class MenuItemSubCategoryAndCategoryViewModel
    {
        public IEnumerable<CategoryModel> CategoryList { get; set; }
        public IEnumerable<SubCategoryModel> SubCategoryList { get; set; }
        public MenuItemModel MenuItem { get; set; }
        public string StatusMessage { get; set; }
    }
}
