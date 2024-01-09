using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class WebSiteMapInfo
    {
        public int? MainID { get; set; }
        public int? LangID { get; set; }
        public string HtmlContent { get; set; }
        public string HotKey { get; set; }
        public string AreaName { get; set; }
        public string Intro { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateName { get; set; }
    }
}
