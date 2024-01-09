using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PageLayoutActivity
    {
        [Key]
        public int LangID { get; set; }
        public string MoreLinkUrl { get; set; }
        public string Item1Link { get; set; }
        public string Item2Link { get; set; }
        public string Item3Link { get; set; }
        public string Item4Link { get; set; }
        public string Item5Link { get; set; }
        public string Item6Link { get; set; }
        public string Item7Link { get; set; }
        public string Item1Desc { get; set; }
        public string Item2Desc { get; set; }
        public string Item3Desc { get; set; }
        public string Item4Desc { get; set; }
        public string Item5Desc { get; set; }
        public string Item6Desc { get; set; }
        public string Item7Desc { get; set; }
        public string Item1ImageName { get; set; }
        public string Item2ImageName { get; set; }
        public string Item3ImageName { get; set; }
        public string Item4ImageName { get; set; }
        public string Item5ImageName { get; set; }
        public string Item6ImageName { get; set; }
        public string Item7ImageName { get; set; }
        public string Item1ImageNamePath { get; set; }
        public string Item2ImageNamePath { get; set; }
        public string Item3ImageNamePath { get; set; }
        public string Item4ImageNamePath { get; set; }
        public string Item5ImageNamePath { get; set; }
        public string Item6ImageNamePath { get; set; }
        public string Item7ImageNamePath { get; set; }
        public string Item1Title { get; set; }
        public string Item2Title { get; set; }
        public string Item3Title { get; set; }
        public string Item4Title { get; set; }
        public string Item5Title { get; set; }
        public string Item6Title { get; set; }
        public string Item7Title { get; set; }
    }
}
