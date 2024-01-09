using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class Users
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? Site_ID { get; set; }
        public int? Group_ID { get; set; }
        public string Account { get; set; }
        public string PWD { get; set; }
        public string User_Name { get; set; }
        public string User_Email { get; set; }
        public bool? Readonly { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
