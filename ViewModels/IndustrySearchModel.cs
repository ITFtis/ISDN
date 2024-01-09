using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class IndustrySearchModel : SearchModelBase
    {
        public IndustrySearchModel()
        {
            Sort = "CompanyName";
        }
        public string Industryid { get; set; }
        public string Industryitemid { get; set; }
        public string UseLang { get; set; }
    }
}
