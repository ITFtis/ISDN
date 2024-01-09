using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class KnowledgeModelColumnSetting
    {
        public int? MainID { get; set; }
        public List<string> ColumnName{ get; set; }
        public List<string> ColumnSettingName { get; set; }
        public List<int> GroupID { get; set; }
        public List<string> GroupName { get; set; }
        public List<string> GroupColumnSettingName { get; set; }
    }
}
