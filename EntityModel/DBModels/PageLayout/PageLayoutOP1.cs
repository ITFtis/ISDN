using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PageLayoutOP1
    {
        [Key]
        public int LangID { get; set; }
        public bool IsShow { get; set; }
        public string LeftTitle { get; set; }
        public string RightTitle { get; set; }
        public string Introduction { get; set; }
        public string RightLink { get; set; }
        public string LeftItem1Link { get; set; }
        public string LeftItem2Link { get; set; }
        public string LeftItem3Link { get; set; }
        public string LeftItem4Link { get; set; }
        public string RightItem1Link { get; set; }
        public string RightItem2Link { get; set; }
        public string RightItem3Link { get; set; }
        public string LeftItem1Desc { get; set; }
        public string LeftItem2Desc { get; set; }
        public string LeftItem3Desc { get; set; }
        public string LeftItem4Desc { get; set; }
        public string RightItem1Desc { get; set; }
        public string RightItem2Desc { get; set; }
        public string RightItem3Desc { get; set; }
        public string LeftItem1ImageName { get; set; }
        public string LeftItem2ImageName { get; set; }
        public string LeftItem3ImageName { get; set; }
        public string LeftItem4ImageName { get; set; }
        public string RightItem1ImageName { get; set; }
        public string RightItem2ImageName { get; set; }
        public string RightItem3ImageName { get; set; }
        public string LeftItem1ImageNamePath { get; set; }
        public string LeftItem2ImageNamePath { get; set; }
        public string LeftItem3ImageNamePath { get; set; }
        public string LeftItem4ImageNamePath { get; set; }
        public string RightItem1ImageNamePath { get; set; }
        public string RightItem2ImageNamePath { get; set; }
        public string RightItem3ImageNamePath { get; set; }
        public string LeftItem1Title { get; set; }
        public string LeftItem2Title { get; set; }
        public string LeftItem3Title { get; set; }
        public string LeftItem4Title { get; set; }
        public string RightItem1Title { get; set; }
        public string RightItem2Title { get; set; }
        public string RightItem3Title { get; set; }
        public string LeftItem1LinkMode { get; set; }
        public string LeftItem2LinkMode { get; set; }
        public string LeftItem3LinkMode { get; set; }
        public string LeftItem4LinkMode { get; set; }
        public string RightItem1LinkMode { get; set; }
        public string RightItem2LinkMode { get; set; }
        public string RightItem3LinkMode { get; set; }
        public string RightLinkMode { get; set; }
    }
}
