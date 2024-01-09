using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ViewModels
{
    public class PageFrontIndexModel : MasterPageModel
    {
        public PageFrontIndexModel() {

        }
        public string ItemID { get; set; }
        public string GroupID { get; set; }
        public string MenuID { get; set; }
        public string Title { get; set; }
        public IList<SelectListItem> ModelItemList { get; set; }
        public string MainID { get; set; }
        public string MenuType { get; set; }
        public string SiteMenuID { get; set; }
        public int ItemCnt { get; set; }
        public int PageItemID { get; set; }
        public string LinkStr { get; set; }
        public string HtmlContent { get; set; }
        public string ImageFileLocation { get; set; }
        public string ImageName { get; set; }
        public string ImageFileDesc { get; set; }
        public bool IsPrint { get; set; }
        public bool IsForward { get; set; }
        public bool IsShare { get; set; }
        public string LinkUrl { get; set; }
        public string DownloadID { get; set; }
        public string DownloadDesc { get; set; }
        public string LinkUrlDesc { get; set; }
        public string UploadFileName { get; set; }
        public string ItemName { get; set; }
    }
}
