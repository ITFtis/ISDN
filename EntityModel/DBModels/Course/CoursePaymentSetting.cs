using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CoursePaymentSetting
    {
        public int MainID { get; set; }
        [Key]
        public int ItemID { get; set; }    
        public bool? CreateCardChk { get; set; }
        public bool? TransferChk { get; set; }
        public bool? CheckChk { get; set; }
        public bool? FreeChk { get; set; }
        public string CreateCardHtml { get; set; }
        public string TransferHtml { get; set; }
        public string CheckHtml { get; set; }
        public string FreeHtml { get; set; }
    }
}


