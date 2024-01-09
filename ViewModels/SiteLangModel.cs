using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
   public  class SiteLangModel
    {
        public SiteLangModel()
        {
            ID = -1;
        }
        public int ID { get; set; }
        public int? Lang_ID { get; set; }
        public int? Site_ID { get; set; }
        public int? Sort { get; set; }
        public int? Area_id { get; set; }
        public string Lang_Name { get; set; }
        public string Disp_Name { get; set; }
        public string Domain_Type { get; set; }
        public string Sub_Domain_Name { get; set; }
        public string Indep_Domain_Name { get; set; }
        public string Content_Source { get; set; }
        public int Link_Lang_ID { get; set; }
        public string Link_Href { get; set; }
    }
}
