using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ForumReplyItem
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? LangID { get; set; }
        public string ReplyContent { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public string CreateDatetimeStr { get; set; }
        public bool? Status { get; set; }
        public int? Replyid { get; set; }
        public int? ReplyCnt { get; set; }
        public bool? isquote { get; set; }
        
    }
}
