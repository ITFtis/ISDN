using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PageIndexItem
    {
        [Key]
        [IsSequence]
        public int? ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? Lang_ID { get; set; }
        public int? Sort { get; set; }
        public string HtmlContent { get; set; }
        public string ItemName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public string ImageFileName { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileDesc { get; set; }
        public string ImageFileLocation { get; set; }
        public string LinkUrl { get; set; }
        public string LinkUrlDesc { get; set; }
        public bool? Enabled { get; set; }
        [EmptyNull]
        public int? ActiveID { get; set; }
        [EmptyNull]
        public int? ActiveItemID { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateName { get; set; }

        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public string CreateName { get; set; }
        public bool? IsVerift { get; set; }
    }
}
