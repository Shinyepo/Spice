using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public List<ShoppingCartModel> ShoppingCart { get; set; }
        public OrderHeaderModel OrderHeader { get; set; }
    }
}
