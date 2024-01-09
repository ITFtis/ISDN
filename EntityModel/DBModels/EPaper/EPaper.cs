using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class EPaper
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? PaperMode { get; set; }
        public int? PaperStyle { get; set; }
        public int? LangID { get; set; }
        public int? Sort { get; set; }
        public string PublicshStr { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string PageEndHtmlContent { get; set; }
        public string TopBannerImgPath { get; set; }
        public string TopBannerImgOrgName { get; set; }
        public string TopBannerImgName { get; set; }
        public string TopHtmlContent { get; set; }
        public string LeftHtmlContent { get; set; }
        public string CenterHtmlContent { get; set; }
        public string BottomHtmlContent { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public bool? IsEdit { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? CreareDatetime { get; set; }
    }
}
