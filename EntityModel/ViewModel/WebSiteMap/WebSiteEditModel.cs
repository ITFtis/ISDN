using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class WebSiteEditModel
    {
        public WebSiteEditModel() {
            MainID = -1;
        }
        public int MainID { get; set; }
        public int LangID { get; set; }
        public string HtmlContent { get; set; }
        public string ModelName { get; set; }
        public string[] HotKey { get; set; }
        public string[] Intro { get; set; }
        public string[] AreaName { get; set; }
    }
}
