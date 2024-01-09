using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ActiveDateRange
    {
        public int? ModelID { get; set; }
        public int? ItemID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
