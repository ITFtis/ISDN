using ResourceLibrary;
using SQLModel.Attributes;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EPaperUnitSettingModel
    {
        public EPaperUnitSettingModel()
        {
            ID = -1;
            ShowCount = 10;
            UnitSettingColumnList = new List<UnitSettingColumn>();
        }
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public bool IsRSS { get; set; }
        public string FrontPagePath { get; set; }
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
        public string LangID { get; set; }
        public Dictionary<string, string> ColumnNameMapping { get; set; }
        public List<UnitSettingColumn> UnitSettingColumnList { get; set; }
        public string[] ColumnName { get; set; }
        public int[] ColumnUse { get; set; }
    }
}
