using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class QuestionnaireCommentResult
    {
        public int QuestionnaireID { get; set; }
        public int Seq { get; set; }
        public string Comment { get; set; }
        public string CommentUser { get; set; }
        public string CreateDatetime { get; set; }
    }
}
