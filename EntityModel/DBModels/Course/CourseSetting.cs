using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseSetting
    {
        [Key]
        public int MainID { get; set; }
        public bool? FrontSearch { get; set; }
        public string ItemCode { get; set; }
        public string SingInRange { get; set; }
        public string Instructions { get; set; }
        public DateTime? Updatetime { get; set; }
        public string UpdateUser { get; set; }
        public string ComplateInstructions { get; set; }
    }
}


