using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class MailSearchModel : SearchModelBase
    {
        public MailSearchModel()
        {
            Sort = "InputDate";
        }
        public string InputDateFrom { get; set; }
        public string InputDateTo { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string KeyWord { get; set; }
        public string Process { get; set; }
        public string Reply { get; set; }
    }
}
