using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ActiveSearchModel : SearchModelBase
    {
        public ActiveSearchModel()
        {
            Sort = "Sort";
            MessageType = "";
        }
        public int? GroupId { get; set; }
        public string MessageType { get; set; }
        public string PublicshDateFrom { get; set; }
        public string PublicshDateTo { get; set; }
        public string DisplayFrom { get; set; }
        public string DisplayTo { get; set; }
        public string Title { get; set; }
        public string Enabled { get; set; }
    }
}
