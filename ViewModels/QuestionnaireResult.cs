using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class QuestionnaireResult
    {
        public int QuestionnaireID { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string CreateDate { get; set; }
        public string LinkGuid { get; set; }
        public string Status { get; set; }
        public string CourseModelItem { get; set; }
        
    }
}
