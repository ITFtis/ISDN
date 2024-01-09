using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class KnowledgeBaseEditModel
    {
        public KnowledgeBaseEditModel() { ID = -1;

        }
        public int ID { get; set; }
        public int GroupID { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EdDate { get; set; }
        public string StDateStr { get; set; }
        public string EdDateStr { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }
        public string Introduction { get; set; }
        public bool MemberDownload { get; set; }
        public string BookName { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }
        public string ImageUrl { get; set; }
        public string PublicshStr { get; set; }
        public string[] Column { get; set; }

        public string WebsiteTitle { get; set; }
        public string Description { get; set; }
        public string[] Keywords { get; set; }
        public string[] ColumnName { get; set; }
        public ColumnSetting[] ColumnSetting { get; set; }
        public bool EnterpriceStudentAuth { get; set; }
        public bool GeneralStudentAuth { get; set; }
        public bool VIPAuth { get; set; }
        public bool EMailAuth { get; set; }
        public bool Enabled { get; set; }
        
    }
}
