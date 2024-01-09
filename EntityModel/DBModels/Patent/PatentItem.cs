using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PatentItem
    {
        [Key]
        [IsSequence]
        public int ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? Lang_ID { get; set; }
        public string Title { get; set; }
        public string Inventor { get; set; }
        public string Field { get; set; }
        public string Year { get; set; }
        public int? ClickCnt { get; set; }
        public int? Link_Mode { get; set; }
        public string HtmlContent { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public string LinkUrl { get; set; }
        public bool? Enabled { get; set; }
        [EmptyNull]
        public DateTime? StDate { get; set; }
        [EmptyNull]
        public DateTime? EdDate { get; set; }
        public DateTime? PublicshDate { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateName { get; set; }
        public bool? IsVerift { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public string CreateName { get; set; }
        public string LinkUrlDesc { get; set; }
        
    }

}
