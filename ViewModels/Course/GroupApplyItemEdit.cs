using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class GroupApplyItemEdit
    {
        public GroupApplyItemEdit()
        {
         
        }
        public string Seq { get; set; }
        public string ItemID { get; set; }
        public string MainID { get; set; }
        public List<string> SIGroupKey { get; set; }
        public List<string> SIGroupColumnName { get; set; }
        public List<string> SIGroupColumnType { get; set; }
        public List<bool> SIGroupUse { get; set; }
        public List<bool> SIGroupMust { get; set; }
        public Dictionary<string, List<string>> SIGroupTableItem { get; set; }
        public Dictionary<string, string> Country { get; set; }
        public Dictionary<string, string> IDentity { get; set; }
        public Dictionary<string, string> Event { get; set; }
        public Dictionary<string, string> SIGroupValue { get; set; }
    }
}
