using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseCheckin
    {
        public int ItemID { get; set; }
        public int ApplyID { get; set; }
        public int Seq { get; set; }
        public string CheckinDate { get; set; }
        public string Type { get; set; }
    }
}


