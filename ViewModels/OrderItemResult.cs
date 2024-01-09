using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class OrderItemResult
    { 
        public string CreateDate { get; set; }
        public string OrderNo { get; set; }
        public string StudentName { get; set; }
        public string PaymentType { get; set; }
        public string LogisticsType { get; set; }
        public string PayPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string InvoiceSend { get; set; }
        public string Status { get; set; }
        public int ID { get; set; }
        public string Invoice { get; set; }
    }
}
