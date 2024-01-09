using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class PatentEditModel
    {
        public PatentEditModel() {
            ItemID = -1;
        }
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public int ItemID { get; set; }
        public int Group_ID { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EdDate { get; set; }
        public string StDateStr { get; set; }
        public string EdDateStr { get; set; }
        public string PublicshStr { get; set; }
        public string Title { get; set; }
        public int? Link_Mode { get; set; }
        public string HtmlContent { get; set; }
        public string Inventor { get; set; }
        public string Field { get; set; }
        public string Year { get; set; }
        public string LinkUrlDesc { get; set; }
        public string LinkUrl { get; set; }
    
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        public string[] Nation { get; set; }
        public string[] Patentno { get; set; }
        public string[] PatentDate { get; set; }
        public string[] EarlyPublicDate { get; set; }
        public string[] EarlyPublicNo { get; set; }
        public string[] Deadline { get; set; }

        public string CreateUser { get; set; }
        public string CreateDatetime { get; set; }
        public string UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string VerifyStatus { get; set; }
        public string VerifyUser { get; set; }
        public string VerifyDateTime { get; set; }


    }
}
