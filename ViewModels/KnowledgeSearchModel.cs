using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class KnowledgeSearchModel : SearchModelBase
    {
        public KnowledgeSearchModel()
        {
            Sort = "Sort";
            SearchItems = "";
        }
        public int? GroupId { get; set; }
        public string SearchItems { get; set; }
    }
}
