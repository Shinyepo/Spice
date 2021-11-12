using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderDetailsSuccessViewModel
    {
        public OrderHeaderModel OrderHeader { get; set; }
        public List<OrderDetailsModel> OrderDetails { get; set; }
    }
}
