using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ForumSearchModel : SearchModelBase
    {
        public ForumSearchModel()
        {
            Sort = "CreateDateTime";
        }
        public int? GroupId { get; set; }
        public string CreateDateFrom { get; set; }
        public string CreateDateTo { get; set; }
        public string Subject { get; set; }
        public string CreateUser { get; set; }
        public string Status { get; set; }
    }
}
