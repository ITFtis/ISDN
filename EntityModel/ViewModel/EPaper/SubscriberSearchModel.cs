using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SubscriberSearchModel:SearchModelBase
    {
        public SubscriberSearchModel()
        {
        }
        public string Account { get; set; }
        public string CHNName { get; set; }
        public string EMail { get; set; }
        public string OPDateTo { get; set; }
        public string OPDateFrom { get; set; }
        public string Status { get; set; }
    }
}
