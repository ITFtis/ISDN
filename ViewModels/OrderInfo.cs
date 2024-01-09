using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class OrderInfo
    {
        public OrderInfo() {
        }
        public string OrderTel { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverTEL { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string CityName { get; set; }
        public string ZIP { get; set; }
        public string Address { get; set; }
        public string InvoiceType { get; set; }
        public string IssueTaxID { get; set; }
        public string InvoiceTitle { get; set; }
        public string ModelItemID { get; set; }
        public string MenuID { get; set; }
        public string HadOldOrder { get; set; }
        
    }
}
