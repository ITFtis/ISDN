using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ClassRecord
    {
        public int StudentID { get; set; }
        public string TaxID { get; set; }
        public string ActiveName { get; set; }
        public string ActiveDate { get; set; }
        public int SignleCount { get; set; }
        public string Price { get; set; }
        public string CouponPrice { get; set; }
    }
}
