using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SearchResult
    {
        public int? ModelID { get; set; }
        public int? ItemID { get; set; }
        public int? MenuID { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
