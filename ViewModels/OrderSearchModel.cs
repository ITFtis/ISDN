using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class OrderSearchModel : SearchModelBase
    {
        public OrderSearchModel()
        {
        }

        public string OrderDateFrom { get; set; }
        public string OrderDateTo { get; set; }
        public string OrderNo { get; set; }
        public string OrderName { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string LogisticsType { get; set; }
        public string OrderStatus { get; set; }
        public string ProductName { get; set; }
    }
}
