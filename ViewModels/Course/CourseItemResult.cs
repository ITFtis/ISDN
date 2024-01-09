using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseItemResult
    {
        public int Seq { get; set; }
        public int ItemID { get; set; }
        public string Title { get; set; }
        public string ClickCount { get; set; }
        public string PublicshDate { get; set; }
        public string  IsRange { get; set; }
        public string GroupName { get; set; }
        public bool? Enabled { get; set; }
        public int Sort { get; set; }
        public string Manager { get; set; }
        public string ActionInfo { get; set; }
        public string ModelName { get; set; }
        public string ActiveStDate { get; set; }
        public string ActiveEdDate { get; set; }
    }
}
