using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseSigninEdit
    {
        public CourseSigninEdit()
        {
            
        }
        public string[] Key { get; set; }
        public string[] ColumnName { get; set; }
        public string[] SaveColumnName { get; set; }
        public string[] ColumnType { get; set; }
        public bool[] Use { get; set; }
        public bool[] Must { get; set; }
        public bool[] SaveUse { get; set; }
        public bool[] SaveMust { get; set; }
        public bool[] BaseMust { get; set; }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public string SigninType { get; set; }
        public Dictionary<string,List<string>> TableItem{ get; set; }

    }

    public class CourseSigninItem
    {
        public CourseSigninItem()
        {

        }
        public string[] K { get; set; }
        public string[] N { get; set; }
        public string[] T { get; set; }
        public bool[] U { get; set; }
        public bool[] M { get; set; }
    }
}
