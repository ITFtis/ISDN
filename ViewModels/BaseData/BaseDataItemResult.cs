using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class BaseDataItemResult
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string ClickCnt { get; set; }
        public string GroupName { get; set; }
        public bool? Enabled { get; set; }
        public int Sort { get; set; }
    }
}
