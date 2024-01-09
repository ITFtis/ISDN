using ResourceLibrary;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{

    public  class PageSeminarModel
    {
        public PageSeminarModel()
        {

        }
        public string ActiveDate { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Link { get; set; }
        public int BGStyle { get; set; }
        public string StyleUploadFileName { get; set; }
        public string StyleUploadFilePath { get; set; }
        public string StyleUploadFileNameOrg { get; set; }
        public string StyleUploadFileUrl { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }
        public string ActiveMonth { get; set; }
    }
}
