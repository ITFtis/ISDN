using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class QuestionnaireReplyModel
    {
        public QuestionnaireReplyModel()
        {
            QuestionnaireID = -1;
            Result = new Dictionary<int, Dictionary<string, int>>();
            TextResult = new Dictionary<int, List<string>>();
        }
        public int QuestionnaireID { get; set; }
        public string Title { get; set; }
        public int ReplyCnt { get; set; }
        public int[] Seq { get; set; }
        public string[] Subject { get; set; }
        public int[] Type { get; set; }
        public string[] TypeItem { get; set; }
        public string[] TypeItemSum { get; set; }
        public Dictionary<int, Dictionary<string, int>> Result { get; set; }
        public Dictionary<int, List<string>> TextResult { get; set; }
    }
}
