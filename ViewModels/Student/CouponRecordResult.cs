using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CouponRecordResult
    {
        public CouponRecordResult()
        {
         
        }
        public string CreateDateTime { get; set; }
        public string CompanyName { get; set; }
        public string Price { get; set; }
        public string Action { get; set; }
        public string Code { get; set; }
        public string CouponCode { get; set; }
        public string Type { get; set; }
    }
}
