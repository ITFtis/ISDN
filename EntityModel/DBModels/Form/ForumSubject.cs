using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ForumSubject
    {
        [Key]
        [IsSequence]
        public int ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? LangID { get; set; }
        public string Subject { get; set; }
        public string SubjectContent { get; set; }
        public int? ClickCnt { get; set; }
        public int? ReplyCnt { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastReplyDateTime { get; set; }
        public string LastReplyer { get; set; }
    }
}
