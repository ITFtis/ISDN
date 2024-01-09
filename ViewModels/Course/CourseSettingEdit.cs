using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseSettingEdit
    {
        public CourseSettingEdit()
        {
            
        }
        public int MainID { get; set; }
        public bool FrontSearch { get; set; }
        public string ItemCode { get; set; }
        public string Instructions { get; set; }
        public string ComplateInstructions { get; set; }
        public int[] From { get; set; }
        public int[] To { get; set; }
        public string[] Text { get; set; }
    }
}
