using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class StudentFormSetting
    {
        public string StudentRight { get; set; }
        public string StudentFinish { get; set; }
        public string StudentForgetPW { get; set; }
        public string StudentJoinUp { get; set; }
        public string StudentJoinDown { get; set; }
        public string CompanyNotice { get; set; }
        public string CouponNotice { get; set; }
        public string Privacy { get; set; }
        public string SigninDesc { get; set; }
        public string OrderDesc { get; set; }
        public string SenderName { get; set; }
        public string SenderEMail { get; set; }
        public string SenderTitle { get; set; }
        public string ReceiveMail { get; set; }
        public string AdminSenderName { get; set; }
        public string AdminSenderEMail { get; set; }
        public string AdminSenderTitle { get; set; }
        public string StudentSenderName { get; set; }
        public string StudentSenderEMail { get; set; }
        public string StudentSenderTitle { get; set; }
    }
}
