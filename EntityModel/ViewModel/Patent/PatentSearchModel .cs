using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PatentSearchModel : SearchModelBase
    {
        public PatentSearchModel()
        {
            Sort = "Sort";
            MessageType = "";
        }
        public int? GroupId { get; set; }
        public string MenuId { get; set; }
        public string MessageType { get; set; }
        public string PublicshDateFrom { get; set; }
        public string PublicshDateTo { get; set; }
        public string DisplayFrom { get; set; }
        public string DisplayTo { get; set; }
        public string Title { get; set; }
        public string Enabled { get; set; }
        public string ListType { get; set; }
        public string IsRange { get; set; }
    }
}
