using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ArticelGroupItem
    {
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public int GroupID { get; set; }
        public int SubGroupID { get; set; }
    }
}
