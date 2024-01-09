using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class BaseDataEditModel
    {
        public BaseDataEditModel()
        {
         
        }
        public int ItemID { get; set; }
        public string GroupID { get; set; }
        public string ItemName { get; set; }
        public string UnitDesc { get; set; }
        public string TimeDesc { get; set; }
        public bool Year { get; set; }
        public bool Month { get; set; }
        public bool Quarter { get; set; }
        public bool Enabled { get; set; }
        public string YearFrom { get; set; }
        public string YearTo { get; set; }
        public string[] Column { get; set; }
        public bool[] ColumnUse { get; set; }
    }
}
