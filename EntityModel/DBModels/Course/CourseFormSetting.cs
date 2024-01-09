using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseFormSetting
    {
        public int MainID { get; set; }
        [Key]
        public int ItemID { get; set; }    
        public bool? RegistrationSingleChk { get; set; }
        public bool? RegistrationGroupChk { get; set; }
        public string RegistrationSingleName { get; set; }
        public string RegistrationGroupName { get; set; }
        public string SingleDesc { get; set; }
        
    }
}


