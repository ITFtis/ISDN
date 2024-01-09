using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class AdminFunctionAuth
    {
        public int GroupID { get; set; }
        public int LangID { get; set; }
        public int ItemID { get; set; }
        public int Type { get; set; }
        public int GID { get; set; }
        
    }
}
