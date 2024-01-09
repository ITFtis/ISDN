using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PageUnitSettingModel
    {
        public PageUnitSettingModel() {
           ID = -1;
        }
        public int? ID { get; set; }
        public int? MainID { get; set; }
        public bool IsPrint { get; set; }
        public bool IsShare { get; set; }
        public bool IsForward { get; set; }
        public bool MemberAuth { get; set; }
        public bool GeneralStudentAuth { get; set; }
        public bool VIPAuth { get; set; }
        public bool EMailAuth { get; set; }
        public bool EnterpriceStudentAuth { get; set; }
        public string Title { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public List<UnitSettingColumn> UnitSettingColumnList { get; set; }
        public string[] ColumnName { get; set; }
        public int[] ColumnUse { get; set; }
        public Dictionary<string, string> ColumnNameMapping { get; set; }
    }
}
