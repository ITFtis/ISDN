using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AdvanceSearchModel : SearchModelBase
    {
        public AdvanceSearchModel()
        {
            Sort = "Sort";
            LangId = "";
            MenuID = "";
            TotalCnt = 0;
            Info = "";
        }
        public string MenuID { get; set; }
        public int TotalCnt { get; set; }
        public string Info { get; set; }
    }
}
