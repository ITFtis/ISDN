using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class SiteLayoutModel
    {
        public SiteLayoutModel() {
            //LogoImgHeight = 80;
            //FirstPageImgHeight = 219;
            //InsidePageImgHeight = 219;
            ID = -1;
        }
        public int? ID { get; set; }
        public string LangID { get; set; }
        public string LogoImgNameOri { get; set; }
        public string LogoImgShowName { get; set; }
        public string LogoImgNameThumb { get; set; }
        public string LogoImageUrl { get; set; }
        public string LogoImageUrlThumb { get; set; }

        public string InnerLogoImgNameOri { get; set; }
        public string InnerLogoImgShowName { get; set; }
        public string InnerLogoImgNameThumb { get; set; }
        public string InnerLogoImageUrl { get; set; }
        public string InnerLogoImageUrlThumb { get; set; }


        //public int? LogoImgHeight { get; set; }
        //public int? FirstPageImgHeight { get; set; }
        //public int? InsidePageImgHeight { get; set; }
        public string FirstPageImgNameOri { get; set; }
        public string FirstPageImgShowName { get; set; }
        public string FirstPageImgNameThumb { get; set; }
        public string FirstPageImageUrl { get; set; }
        public string InsidePageImgNameOri { get; set; }
        public string InsidePageImgShowName { get; set; }
        public string InsidePageImgNameThumb { get; set; }
        public string InsidePageImageUrl { get; set; }
        public HttpPostedFileBase LogoImageFile { get; set; }
        public HttpPostedFileBase InnerLogoImageFile { get; set; }
        public HttpPostedFileBase FirstPageImageFile { get; set; }
        public HttpPostedFileBase InsidePageImageFile { get; set; }
        public HttpPostedFileBase FowardImageFile { get; set; }
        public HttpPostedFileBase PrintImageFile { get; set; }
        public string  HtmlContent { get; set; }

        public string FowardImgNameOri { get; set; }
        public string FowardImgShowName { get; set; }
        public string FowardImgNameThumb { get; set; }
        public string FowardImageUrl { get; set; }
        public string FowardHtmlContent { get; set; }

        public string PrintImgNameOri { get; set; }
        public string PrintImgShowName { get; set; }
        public string PrintImgNameThumb { get; set; }
        public string PrintImageUrl { get; set; }
        public string PrintHtmlContent { get; set; }

        public string Page404HtmlContent { get; set; }
        public string Page404Title { get; set; }

        public string SType { get; set; }
        public string PublishContent { get; set; }
        
    }
    
}
