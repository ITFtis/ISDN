
using SQLModel.Models;
using System;
using System.Collections.Generic;

namespace ViewModels
{
    public class MailInputModel
    {
        public MailInputModel() {
            InputKey = new List<string>();
            InputValue = new List<string>();
        }
     
        public int ID { get; set; }
        public int MainID { get; set; }
        public DateTime CreateDatetime { get; set; }
        public IList<string> InputKey { get; set; }
        public IList<string> InputValue { get; set; }
        public string Progress { get; set; }
        public FormInputNote[] ProcessNote { get; set; }
        public FormInputNote[] ReplyNote { get; set; }
    }
}
