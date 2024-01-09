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
        public string Key2 { get; set; }
        public string Key3 { get; set; }
        public string Sel1 { get; set; }
        public string Sel2 { get; set; }
        public string SearchType { get; set; }
        public string MenuType { get; set; }
        public string Menu1 { get; set; }
        public string Menu2 { get; set; }
        public string Menu3 { get; set; }
    }
}
