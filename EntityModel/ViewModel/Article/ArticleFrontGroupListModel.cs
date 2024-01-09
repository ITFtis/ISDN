using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ViewModels
{
    public class ArticleFrontGroupListModel : MasterPageModel
    {
        public ArticleFrontGroupListModel() {

        }
        public string MainID { get; set; }
        public string MenuID { get; set; }
        public string Title { get; set; }
        public IList<GroupArticle> GroupArticle { get; set; }
        public IList<GroupItemArticle> SubGroupArticle { get; set; }
        public bool IsPrint { get; set; }
        public bool IsForward { get; set; }
        public bool IsShare { get; set; }
        public string MenuType { get; set; }
        public string SiteMenuID { get; set; }
        public string LinkStr { get; set; }
    }
}
