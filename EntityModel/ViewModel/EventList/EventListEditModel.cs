using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class EventListEditModel
    {
        public EventListEditModel() {
            ItemID = -1;
            PublicshStr = DateTime.Now.ToString("yyyy/MM/dd");
            Title = "";
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
        public string LinkUrl { get; set; }
        public string ActiveID { get; set; }
        public string ActiveItemID { get; set; }

        public string ImageUrl { get; set; }
        public string ImageFileDesc { get; set; }
        public string ImageFileLocation { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }

        
        public string RelateImagelUrl { get; set; }
        public HttpPostedFileBase RelateImageFile { get; set; }
        public string RelateImageFileOrgName { get; set; }
        public string RelateImageName { get; set; }


        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        public string WebsiteTitle { get; set; }
        public string Description { get; set; }
        public string[] Keywords { get; set; }
        public string Introduction { get; set; }
        public string VerifyStatus { get; set; }
        public string VerifyUser { get; set; }
        public string VerifyDateTime { get; set; }
        public string CreateUser { get; set; }
        public string CreateDatetime { get; set; }
        public string UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string LinkUrlDesc { get; set; }
        public string Year { get; set; }
    }
}
