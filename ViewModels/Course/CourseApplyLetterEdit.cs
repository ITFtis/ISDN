﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseApplyLetterEdit
    {
        public CourseApplyLetterEdit()
        {
         
        }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public string SigninDesc { get; set; }
        public string PaymentDesc { get; set; }
        public string CourseDesc { get; set; }
        public string QuestionnaireDesc { get; set; }
        public string SenderName { get; set; }
        public string SenderEMail { get; set; }
        public string SenderTitle { get; set; }
        public string ReceiveMail { get; set; }
        public string AdminSenderName { get; set; }
        public string AdminSenderEMail { get; set; }
        public string AdminSenderTitle { get; set; }
     
    }
}
