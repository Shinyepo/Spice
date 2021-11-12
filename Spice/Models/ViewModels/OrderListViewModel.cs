using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderListViewModel
    {
        public IList<OrderDetailsSuccessViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
