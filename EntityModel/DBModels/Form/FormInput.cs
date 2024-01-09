using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class FormInput
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? MainID { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string JSONStr { get; set; }
        public string Progress { get; set; }
        public string ProcessAccount { get; set; }
        public string ReplyAccount { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? ProcessDatetime { get; set; }
        public DateTime? ReplyDatetime { get; set; }
    }
}