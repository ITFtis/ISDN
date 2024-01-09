using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ActiveItem
    {
        [Key]
        [IsSequence]
        public int ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? Lang_ID { get; set; }
        public string Title { get; set; }
        public int? ClickCnt { get; set; }
        public int? Link_Mode { get; set; }
        public string HtmlContent { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public string ImageFileName { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileDesc { get; set; }
        public string ImageFileLocation { get; set; }
        public string LinkUrl { get; set; }
        public bool? Enabled { get; set; }
        [EmptyNull]
        public DateTime? StDate { get; set; }
        [EmptyNull]
        public DateTime? EdDate { get; set; }
        public DateTime? PublicshDate { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        [EmptyNull]
        public int? ActiveID { get; set; }
        [EmptyNull]
        public int? ActiveItemID { get; set; }

        public string RelateImageFileName { get; set; }
        public string RelateImageFileOrgName { get; set; }
        public string Introduction { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string CreateName { get; set; }
        public string UpdateName { get; set; }
        public bool? IsVerift { get; set; }
        public string LinkUrlDesc { get; set; }
        [EmptyNull]
        public DateTime? StDateNew { get; set; }
        [EmptyNull]
        public DateTime? EdDateNew { get; set; }
        public bool? IsNew { get; set; }
        public string Organizer { get; set; }
        [EmptyNull]
        public DateTime? ActiveDate { get; set; }
        public string Location { get; set; }
        public string Info { get; set; }
        public string Price { get; set; }
        public string Way { get; set; }
        public string WebSite { get; set; }
        public string Search { get; set; }
        [EmptyNull]
        public DateTime? StSignDate { get; set; }
        [EmptyNull]
        public DateTime? EdSignDate { get; set; }
        public string Contact { get; set; }
        public string ContactMail { get; set; }
        public string ContactFax { get; set; }
        public string Source { get; set; }
   
        
    }
}
