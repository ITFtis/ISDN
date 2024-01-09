using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CountSearchKey
    {
        public string SearchKey { get; set; }
        public int Count { get; set; }
        public int LangID { get; set; }
    }
}
