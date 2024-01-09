using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SaveSignleApplySaveEdit
    {
        public SaveSignleApplySaveEdit()
        {
            
        }
        public int ID { get; set; }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public string[] CouponItem { get; set; }
        public string[] CouponPrice { get; set; }
        public Dictionary<string,string> BCValue { get; set; }
        public Dictionary<string, string> SIValue { get; set; }
        public string Price { get; set; }
        public string PayPrice { get; set; }
        public string PaymentType { get; set; }
        public List<Dictionary<string, string>> SIGroupValue { get; set; }
        public string[] groupSeqIndex { get; set; }
        public string MenuID { get; set; }
        public string[] InfoPrice { get; set; }
        public string StudentID { get; set; }
        public string CustomerPrice { get; set; }
        public string Status { get; set; }
        public string CourseDesc { get; set; }
    }

}
