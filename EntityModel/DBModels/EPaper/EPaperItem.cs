using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class EPaperItem
    {
        public int EPaperID { get; set; }
        public int ItemID { get; set; }
        public int ModelID { get; set; }
        public int MenuID { get; set; }
        public int MainID { get; set; }
        public int Sort { get; set; }
        public int MenuLevel3Sort { get; set; }
        public int MenuLevel2Sort { get; set; }
        public int MenuLevel1Sort { get; set; }
        public int GroupSortID { get; set; }
    }
}
