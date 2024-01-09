using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class BaseDataInputModel
    {
        public BaseDataInputModel()
        {
         
        }
        public string ItemID { get; set; }
        public string DataID { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Quarter { get; set; }
        public string[] Text { get; set; }
        public bool[] Use { get; set; }
    }
}
