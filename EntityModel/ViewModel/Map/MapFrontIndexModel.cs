using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ViewModels
{
    public class MapFrontIndexModel : MasterPageModel
    {
        public MapFrontIndexModel() {

        }
        public string ItemID { get; set; }
        public string GroupID { get; set; }
        public string MenuID { get; set; }
        public string Title { get; set; }
        public string MainID { get; set; }
        public string LinkStr { get; set; }
        public IList<SQLModel.Models.Menu> UpMenulist { get; set; }
        public IList<SQLModel.Models.Menu> MainMenulist { get; set; }
        public IList<SQLModel.Models.Menu> DownMenulist { get; set; }
        public WebSiteEditModel EditInfo { get; set; }
    }
}
