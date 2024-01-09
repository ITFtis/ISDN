using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class EPaperEditModel
    {
        public EPaperEditModel() {
            ID = -1;
            PublicshStr = DateTime.Now.ToString("yyyy/MM/dd");
        }
        public int ID { get; set; }
        public int PaperMode { get; set; }
        public int PaperStyle { get; set; }
        public string PublicshStr { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
      
        public string TopBannerImgUrl { get; set; }
        public string TopBannerImgPath { get; set; }
        public string TopBannerImgOrgName { get; set; }
        public string TopBannerImgName { get; set; }
        public HttpPostedFileBase TopBannerImg { get; set; }

        public string PageEndHtmlContent { get; set; }
        public string TopHtmlContent { get; set; }
        public string LeftHtmlContent { get; set; }
        public string CenterHtmlContent { get; set; }
        public string BottomHtmlContent { get; set; }

        public HttpPostedFileBase[] ADImageFiles { get; set; }
        public string[] ADName { get; set; }
        public string[] ADLink { get; set; }
        public string[] ADID { get; set; }
        public string[] ADFileName { get; set; }
        public string[] ADFilePath { get; set; }
        public string EPaperContent { get; set; }
        public List<EPaperItemEdit> EPaperItemEdit { get; set; }

        public bool Enabled { get; set; }

    }
    public class EPaperItemEdit
    {
        public string Name { get; set; }
        public List<string> ItemName { get; set; }
        public List<string> ItemUrl { get; set; }
    }
 }



