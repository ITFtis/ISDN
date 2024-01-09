using ResourceLibrary;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class HomeViewModel: MasterPageModel
    {
        public HomeViewModel()
        {

        }
        public IList<LinkItemResult> ActiveLinkItems { get; set; }
        public IList<LinkItemResult> NewsLinkItems { get; set; }
        public PageSeminarModel PageSeminarModel { get; set; }
        public HomePageLayoutModel PageLayoutModel1 { get; set; }
        public HomePageLayoutModel PageLayoutModel2 { get; set; }
        public HomePageLayoutModel PageLayoutModel3 { get; set; }
    }

    public  class HomePageLayoutModel
    {
        public HomePageLayoutModel()
        {
            ID = -1;
            LinkUrl = new List<string>();
            LinkImageSrc = new List<string>();
            PublicshDate = new List<string>();
            HtmlContent = new List<string>();
            ModelGroup = new List<string>();
            LinkImageDesc = new List<string>();
            JustView = new List<bool>();
            IsNew = new List<bool>();
            Title = new List<string>();
        }
        public int ID { get; set; }
        public int MenuID { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string ImageOri { get; set; }
        public int ModelID { get; set; }
        public int MainID { get; set; }
        public List<string> Title { get; set; }
        public List<string> LinkUrl { get; set; }
        public List<string> LinkImageSrc { get; set; }
        public List<string> LinkImageDesc { get; set; }
        public List<string> PublicshDate { get; set; }
        public List<string> HtmlContent { get; set; }
        public List<string> ModelGroup { get; set; }
        public List<bool> JustView { get; set; }
        public List<bool> IsNew { get; set; }
        public string MoreLink { get; set; }
    }
}
