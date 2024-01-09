using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ForumSubjectResult
    {
        public int ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? LangID { get; set; }
        public string Subject { get; set; }
        public string CreateUser { get; set; }
        public int? ClickCnt { get; set; }
        public int? ReplyCnt { get; set; }
        public string GroupName { get; set; }
        public string CreateDateTime { get; set; }
        public string LastReplyDateTime { get; set; }
    }
}
