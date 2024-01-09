using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseBasicColumnEdit
    {
        public CourseBasicColumnEdit()
        {
            
        }
        public string[] Key { get; set; }
        public string[] ColumnName { get; set; }
        public string[] ColumnType { get; set; }
        public bool[] SingalUse { get; set; }
        public bool[] GroupUse { get; set; }
        public bool[] SingalMust { get; set; }
        public bool[] GroupMust { get; set; }
        public bool[] BaseMust { get; set; }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public Dictionary<string,List<string>> TableItem{ get; set; }
    }

    public class CourseBasicColumnItem
    {
        public CourseBasicColumnItem()
        {

        }
        public string[] K { get; set; }
        public string[] N { get; set; }
        public string[] T { get; set; }
        public bool[] SU { get; set; }
        public bool[] GU { get; set; }
        public bool[] SM { get; set; }
        public bool[] GM{ get; set; }
    }
}
