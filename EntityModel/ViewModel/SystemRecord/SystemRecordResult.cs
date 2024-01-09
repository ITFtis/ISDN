using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SystemRecordResult
    {
        public string TempID { get; set; }
        public int UserID { get; set; }
        public string Account { get; set; }
        public string IP { get; set; }
        public string Login { get; set; }
        public string Logout { get; set; }
    }
}
