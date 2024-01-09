using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class FirstPageModel
    {
        public FirstPageModel() {
            ADMain = new List<ADBase>();
        }
        public string AdminHost { get; set; }
        public string LogoUrl { get; set; }
        public string LogoHeight { get; set; }
        public IList<ADBase> ADMain { get; set; }
        public IList<ADBase> ADCenter { get; set; }
        public IList<ADBase> ADRight { get; set; }
        public IList<ADBase> ADRightDown { get; set; }
        public IList<ADBase> ADDown { get; set; }
        public string TopMenu { get; set; }
        public string LeftMenu { get; set; }
        public string MainMenu { get; set; }
        public string FooterMenu { get; set; }
        public string FooterString { get; set; }
        public string SearchKey { get; set; }
        public string[] SEOScript { get; set; }
        public string IsShowSearch { get; set; }
        public int ShowModel { get; set; }
        public string BannerImage { get; set; }
        public string Device { get; set; }
    }
}
