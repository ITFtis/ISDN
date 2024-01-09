using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class QuestionnaireEditModel
    {
        public QuestionnaireEditModel()
        {
            QuestionnaireID = -1;
            DeleteUploadFile = "N";
        }
        public int QuestionnaireID { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string LinkGuid { get; set; }
        public int[] Seq { get; set; }
        public string[] Subject { get; set; }
        public string[] MustInput { get; set; }
        public int[] Type { get; set; }
        public string[] TypeItem { get; set; }
        public string CreateDate { get; set; }
        public string BuildingUser { get; set; }
        public string UploadFileDesc { get; set; }
        public string Descrption { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        public string DeleteUploadFile { get; set; }
        public bool HasDescrptionContent { get; set; }
        public int Status { get; set; }
    }
}
