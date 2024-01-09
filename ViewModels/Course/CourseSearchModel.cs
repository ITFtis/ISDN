using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseSearchModel : SearchModelBase
    {
        public CourseSearchModel()
        {
            Sort = "Sort";
        }
        public int? GroupId { get; set; }
        public string PublicshDateFrom { get; set; }
        public string PublicshDateTo { get; set; }
        public string DisplayFrom { get; set; }
        public string DisplayTo { get; set; }
        public string DisplayRange { get; set; }
        public string ActiveFrom { get; set; }
        public string ActiveTo { get; set; }
        public string Title { get; set; }
        public string Enabled { get; set; }
        public string Range { get; set; }
        public string SearchDetail { get; set; }
        public string Area { get; set; }
        public string PayType { get; set; }
        public string GroupEnabled { get; set; }
    }
}
