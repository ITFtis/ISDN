using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class BaseDataGroupSetting
    {
        public int MainID { get; set; }
        public int LangID { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; }
    }
}
