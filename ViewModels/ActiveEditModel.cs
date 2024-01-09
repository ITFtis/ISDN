using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class ActiveEditModel
    {
        public ActiveEditModel() {
            ItemID = -1;
            PublicshStr = DateTime.Now.ToString("yyyy/MM/dd");
        }
        public int ModelID { get; set; }
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
        public string ImageUrl { get; set; }
        public string ImageFileDesc { get; set; }
        public string ImageFileLocation { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }

        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        public string WebsiteTitle { get; set; }
        public string Description { get; set; }
        public string[] Keywords { get; set; }

        public HttpPostedFileBase CoverImageFile { get; set; }
        public string CoverImageUrl { get; set; }
        public string CoverImageFileOrgName { get; set; }
        public string CoverImageFileName { get; set; }

    }
}
