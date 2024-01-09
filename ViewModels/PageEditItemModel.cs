using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class PageEditItemModel
    {
        public PageEditItemModel() { ItemID = -1;
        }
        public int? ItemID { get; set; }
        public int? ModelID { get; set; }

        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }
        public string LinkUrl { get; set; }
     
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }
        public string ImageFileDesc { get; set; }
        public string ImageFileLocation { get; set; }
        public string WebsiteTitle { get; set; }
        public string Description { get; set; }
        public string[] Keywords { get; set; }
        public string ImageUrl { get; set; }
        public string HtmlContent { get; set; }
        public string ActiveID { get; set; }
        public string ActiveItemID { get; set; }
        public string LangID { get; set; }
    }
}
