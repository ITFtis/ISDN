using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CoursePrice
    {
        [Key]
        public int ItemID { get; set; }
        public int MainID { get; set; }
        public int PriceType { get; set; }
        public bool? SettingPriceChk { get; set; }
        public bool? DiscountRangeChk { get; set; }
        public bool? GroupPriceChk { get; set; }
        [EmptyNull]
        public DateTime? StDate { get; set; }
        [EmptyNull]
        public DateTime? EdDate { get; set; }
        public string GroupPrice { get; set; }
        public DateTime? Updatetime { get; set; }
        public string UpdateUser { get; set; }
        public string CoursePriceItem { get; set; }
        
    }
}


