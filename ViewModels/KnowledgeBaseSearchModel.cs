using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class KnowledgeBaseSearchModel : SearchModelBase
    {
        public KnowledgeBaseSearchModel()
        {
            Sort = "Sort";
            BookName = "";
        }
        public string GroupId { get; set; }
        public string BookName { get; set; }
        public string Enabled { get; set; }
    }
}
