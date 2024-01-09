using ResourceLibrary;
using SQLModel.Attributes;
using SQLModel.Models;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CartUnitSettingModel
    {
        public CartUnitSettingModel()
        {
            ID = -1;
            ShowCount = 10;
            ColumnNameMapping = new Dictionary<string, string>();
        }
        public int? ID { get; set; }
        public int? MainID { get; set; }
        public bool IsPrint { get; set; }
        public bool IsForward { get; set; }
        public bool IsRSS { get; set; }
        public bool IsShare { get; set; }
        public bool MemberAuth { get; set; }
        public bool GeneralStudentAuth { get; set; }
        public bool VIPAuth { get; set; }
        public bool EMailAuth { get; set; }
        public bool EnterpriceStudentAuth { get; set; }
        public int? ShowCount { get; set; }
        public int? ProductShowModel { get; set; }
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
        public string Column22 { get; set; }
        public string Column23 { get; set; }
        public string Column24 { get; set; }
        public string Column25 { get; set; }
        public string Column26 { get; set; }
        public string Column27 { get; set; }
        public string Column28 { get; set; }
        public string Column29 { get; set; }
        public string Column30 { get; set; }
        public string Column31 { get; set; }
        public string Column32 { get; set; }
        public string Column33 { get; set; }
        public string Column34 { get; set; }
        public string Column35 { get; set; }
        public string Column36 { get; set; }
        public string Column37 { get; set; }
        public string Column38 { get; set; }
        public string Column39 { get; set; }
        public string Column40 { get; set; }
        public string Column41 { get; set; }
        public string Column42 { get; set; }
        public string Column43 { get; set; }
        public string Column44 { get; set; }
        public string Column45 { get; set; }
        public string Column46 { get; set; }
        public string Column47 { get; set; }
        public string Column48 { get; set; }
        public string Column49 { get; set; }
        public string Column50 { get; set; }
        public string Column51 { get; set; }
        public string Column52 { get; set; }
        public string Column53 { get; set; }
        public string Column54{ get; set; }
        public string Column55 { get; set; }
        public string Column56 { get; set; }
        public Dictionary<string, string> ColumnNameMapping { get; set; }
        public LangKeyUnit[] LangKeyUnits { get; set; }
    }
}
