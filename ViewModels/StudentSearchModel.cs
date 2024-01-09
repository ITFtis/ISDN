using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class StudentSearchModel : SearchModelBase
    {
        public string Account { get; set; }
        public string CHNName { get; set; }
        public string EMail { get; set; }
        public string VIP { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
        public string VIPDateFrom { get; set; }
        public string VIPDateTo { get; set; }
        public string OrderEPaper { get; set; }
    }
}
