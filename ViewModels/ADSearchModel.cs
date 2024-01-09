using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ADSearchModel : SearchModelBase
    {
        public ADSearchModel()
        {
            Sort = "Sort";
            ADType = "";
            AD_Name = "";
            Lang_ID = "1";
        }
        public string ADType { get; set; }
        public string AD_Name { get; set; }
        public string Lang_ID { get; set; }
        public string SType { get; set; }
    }
}
