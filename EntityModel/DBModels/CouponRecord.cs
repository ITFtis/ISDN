using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class CouponRecord
    {
      
        public DateTime CreateDateTime { get; set; }
        public string StudentID { get; set; }
        public string TaxID { get; set; }
        public string CompanyName { get; set; }
        public string Price { get; set; }
        public string Action { get; set; }
        public string Code { get; set; }
        public string CouponCode { get; set; }
        public string Type { get; set; }
        public string SourceID { get; set; }
    }
}
