using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ADListResult
    {
        public int? ID { get; set; }
        public int? Lang_ID { get; set; }
        public int? Site_ID { get; set; }
        public int? Sort { get; set; }
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
        public bool? Fixed { get; set; }
        public string IsRange { get; set; }
        public bool? Enabled { get; set; }
        
    }
}
