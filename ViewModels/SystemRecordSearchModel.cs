using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SystemRecordSearchModel : SearchModelBase
    {
        public SystemRecordSearchModel()
        {
        }
        public string LoginDateFrom { get; set; }
        public string LoginDateTo { get; set; }
        public string LogoutDateFrom { get; set; }
        public string LogoutDateTo { get; set; }
        public string Account { get; set; }
        public string IP { get; set; }
    }
}
