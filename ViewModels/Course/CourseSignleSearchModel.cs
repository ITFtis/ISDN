using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseSignleSearchModel : SearchModelBase
    {
        public CourseSignleSearchModel()
        {
            Sort = "ID";
        }
        public int ItemID { get; set; }
        public int StudentID { get; set; }
        public string SNCode { get; set; }
        public string ApplyName { get; set; }
        public string CName { get; set; }
        public string SType { get; set; }
        public string Status { get; set; }
        public string PStatus { get; set; }
        public string ActiveFrom { get; set; }
        public string ActiveTo { get; set; }
    }
    
}
