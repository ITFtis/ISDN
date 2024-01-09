using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseSignin
    {
        [Key]
        public int ItemID { get; set; }
        public int MainID { get; set; }
        public string ColumnItems { get; set; }
        public string ColumnSetting { get; set; }
        public DateTime? Updatetime { get; set; }
        public string UpdateUser { get; set; }
        [Key]
        public string SigninType { get; set; }
    }
}


