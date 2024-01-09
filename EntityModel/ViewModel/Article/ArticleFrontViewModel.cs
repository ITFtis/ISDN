using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ViewModels
{
    public class ArticleFrontViewModel : MasterPageModel
    {
        public ArticleFrontViewModel() {

        }
        public string MainID { get; set; }
        public string ItemID { get; set; }
        public string GroupID { get; set; }
        public string MenuID { get; set; }
        public string Title { get; set; }
        public string MainTitle { get; set; }
        public string GroupName { get; set; }
        public string Content { get; set; }
        public bool IsPrint { get; set; }
        public bool IsForward { get; set; }
        public bool  IsShare { get; set; }
        public string ImageName { get; set; }
        public string ImageFileLocation { get; set; }
        public string PublicshDate { get; set; }
        public string ImageFileDesc { get; set; }
        public string LinkUrl { get; set; }
        public string DownloadID { get; set; }
        public string DownloadDesc { get; set; }
        public string MenuType { get; set; }
        public string SiteMenuID { get; set; }
        public string LinkStr { get; set; }
        public string ArticleLink { get; set; }
        public string ArticleMore { get; set; }
        public bool ArticleHasMore { get; set; }
        public string ArticleMoreNoScript { get; set; }
        public string LinkUrlDesc { get; set; }
        public string UploadFileName { get; set; }
        public List<ExtItemObj> ExtItem { get; set; }
        public List<string> GroupNameList { get; set; }
    }
}
