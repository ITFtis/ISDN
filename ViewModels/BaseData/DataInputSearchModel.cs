using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DataInputSearchModel : SearchModelBase
    {
        public DataInputSearchModel()
        {
            Sort = "ID";
        }
        public string ItemID { get; set; }
        public string Type { get; set; }
        public string Display { get; set; }
        public string Input { get; set; }
        public string YearFrom { get; set; }
        public string YearTo { get; set; }
        public string MonthFrom { get; set; }
        public string MonthTo { get; set; }
        public string QuarterFrom { get; set; }
        public string QuarterTo { get; set; }
        public string AllColumn { get; set; }
    }
}
