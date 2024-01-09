using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class EPaperContent
    {
        public int EID { get; set; }
        public string EPaperHtmlContent { get; set; }
    }
}
