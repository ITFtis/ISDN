using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseFormEdit
    {
        public CourseFormEdit()
        {
            
        }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public bool RegistrationSingleChk { get; set; }
        public bool RegistrationGroupChk { get; set; }
        public string RegistrationSingleName { get; set; }
        public string RegistrationGroupName { get; set; }
        public string SingleDesc { get; set; }
    }
}
