using ResourceLibrary;
using SQLModel.Attributes;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PageIndexSettingModel
    {
        public PageIndexSettingModel() { ID = -1;
            ShowCount = 10;
        }
        public int? ID { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsInPage { get; set; }
        public int? ShowCount { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }
        public string Column8 { get; set; }
        public string Column9 { get; set; }
        public string Column10 { get; set; }
        public string Column11 { get; set; }
        public string Column12 { get; set; }
        public string Column13 { get; set; }
        public string Column14 { get; set; }
        public string Column15 { get; set; }
        public string Column16 { get; set; }
        public string Column17 { get; set; }
        public string Column18 { get; set; }
        public string Column19 { get; set; }
        public string Column20 { get; set; }
        public string Column21 { get; set; }
        public string HtmlContent { get; set; }
        public string HtmlContentCode { get; set; }
        public Dictionary<string, string> ColumnNameMapping { get; set; }
    }
}
