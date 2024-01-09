using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PageLayoutOP2
    {
        [Key]
        public int LangID { get; set; }
        public bool IsShow { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Link { get; set; }
        public string ImageName { get; set; }
        public string ImageNamePath { get; set; }
        public string LinkMode { get; set; }
    }
}
