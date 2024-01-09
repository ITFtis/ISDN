using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class MenuEditModel
    {
        public MenuEditModel() {
            LinkMode = 2;
            ImageHeight = 219;
            ParentID = -1;
        }
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public int LangID { get; set; }
        public int ModelID { get; set; }
        public int MenuType { get; set; }
        public int ModelItemID { get; set; }
        public int MenuLevel { get; set; }
        public int? LinkMode { get; set; }
        public int? ShowMode { get; set; }
        public int? OpenMode { get; set; }
        public string MenuName { get; set; }
        public int? ImageHeight { get; set; }
        public int? WindowWidth { get; set; }
        public int? WindowHeight { get; set; }
        public string ImgNameOri { get; set; }
        public string ImgShowName { get; set; }
        public string ImgNameThumb { get; set; }

        public string ImageUrl { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string LinkUrl { get; set; }

        public HttpPostedFileBase LinkUploadFile { get; set; }
        public string LinkUploadFileName { get; set; }
        public string LinkUploadFilePath { get; set; }
        public string DeleteUploadFile { get; set; }

        public string DisplayName { get; set; }
        public string ICon { get; set; }
    }
    
}
