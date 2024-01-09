using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class SiteLayout
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? LangID { get; set; }
        public string LogoImgNameOri { get; set; }
        public string LogoImgShowName { get; set; }
        public string LogoImgNameThumb { get; set; }
        public int? LogoImgHeight { get; set; }
        public string FirstPageImgNameOri { get; set; }
        public string FirstPageImgShowName { get; set; }
        public string FirstPageImgNameThumb { get; set; }
        public int? FirstPageImgHeight { get; set; }
        public string InsidePageImgNameOri { get; set; }
        public string InsidePageImgShowName { get; set; }
        public string InsidePageImgNameThumb { get; set; }
        public int? InsidePageImgHeight { get; set; }
        public string HtmlContent { get; set; }

        public string FowardImgNameOri { get; set; }
        public string FowardImgShowName { get; set; }
        public string FowardImgNameThumb { get; set; }
        public string FowardHtmlContent { get; set; }

        public string PrintImgNameOri { get; set; }
        public string PrintImgShowName { get; set; }
        public string PrintImgNameThumb { get; set; }
        public string PrintHtmlContent { get; set; }
        public string Page404HtmlContent { get; set; }
        public string Page404Title { get; set; }
        public string SType { get; set; }

        public string InnerLogoImgNameOri { get; set; }
        public string InnerLogoImgShowName { get; set; }
        public string InnerLogoImgNameThumb { get; set; }
        public int? InnerLogoImgHeight { get; set; }
        public string PublishContent { get; set; }
        public string ActiveMoreInfo { get; set; }
        public string NewsMoreInfo { get; set; }

        
    }
}
