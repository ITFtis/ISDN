using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class ADEditModel 
    {
        public ADEditModel() { ID = -1;

        }
        public int ID { get; set; }
        public int Lang_ID { get; set; }
        public int Site_ID { get; set; }
        public int Sort { get; set; }
        public string Type_ID { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EdDate { get; set; }
        public string AD_Name { get; set; }
        public string Img_Name_Ori { get; set; }
        public string Img_Name_Thumb { get; set; }
        public int? AD_Width { get; set; }
        public int? AD_Height { get; set; }
        public string Link_Href { get; set; }
        public string Link_Mode { get; set; }
        public bool Fixed { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string Type { get; set; }
        public String UseFileName { get; set; }
        public String ImageUrl { get; set; }
        public string StDateStr { get; set; }
        public string EdDateStr { get; set; }
        public string Img_Show_Name { get; set; }
        public string SType { get; set; }
    }
}
