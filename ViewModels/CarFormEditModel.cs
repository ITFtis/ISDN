using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CarFormEditModel
    {
        public CarFormEditModel()
        {
         
        }
        public string MainID { get; set; }
        public string Step1Html { get; set; }
        public string Step2Html { get; set; }
        public string Step3Html { get; set; }
        public string Step4Html { get; set; }
        public string Step5Html { get; set; }
        public string OrderNoticeHtml { get; set; }
        public string ShoppingSenderName { get; set; }
        public string ShoppingSenderEMail { get; set; }
        public string ShoppingSenderTitle { get; set; }
    }
}
