using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class FormSettingModel
    {
        public FormSettingModel()
        {
            ID = -1;
            ItemID = -1;
        }
        public int ID { get; set; }
        public int ItemID { get; set; } 
        public string FormDesc { get; set; }
        public string ConfirmContent { get; set; }
        public string SenderName { get; set; }
        public string SenderEMail { get; set; }
        public string SenderTitle { get; set; }
        public string ReceiveMail { get; set; }
        public string AdminSenderName { get; set; }
        public string AdminSenderEMail { get; set; }
        public string AdminSenderTitle { get; set; }



    }
}
