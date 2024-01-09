using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class SiteConfigModel
    {
        public SiteConfigModel() {
            ID = -1;
            Login_Title = "產業永續發展整合資訊網";
        }
        public int? ID { get; set; }
        public string Login_Title { get; set; }
        public string Comp_Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public string Page_Title { get; set; }
        public string MailServerIP { get; set; }
        public int Port { get; set; }
        public string EMailAccount { get; set; }
        public string EMailPassword { get; set; }
        public bool IsAuthMailServer { get; set; }
        public int FirstPageMaxLinkItem { get; set; }
        public string LoginDescContent { get; set; }
        public string LogoUploadFileName { get; set; }
        public string LogoUploadFilePath { get; set; }
        public string LogoUploadFileDesc { get; set; }
        public string LogoFileUrl { get; set; }
        public string LogoUploadFileNameOrg { get; set; }
        public HttpPostedFileBase LogoUploadFile { get; set; }
    }
}
