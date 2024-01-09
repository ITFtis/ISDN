using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class ForumGroupEditModel
    {
        public ForumGroupEditModel() { GroupID = -1;}
        public int ModelID { get; set; }
        public int GroupID { get; set; }
        public int MainID { get; set; }
        public string GroupName { get; set; }
        public string GroupDesc { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }
        public string ImgNameThumb { get; set; }
        public string ImageUrl { get; set; }
        public bool Enabled { get; set; }
        
    }
}
