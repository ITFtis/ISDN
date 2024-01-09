using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class LangInputText
    {
        public int LangID { get; set; }
        public int LangTextID { get; set; }
        public string Text { get; set; }
    }
}