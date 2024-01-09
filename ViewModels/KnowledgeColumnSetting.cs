using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class KnowledgeColumnSetting
    {
        public string Lang_ID { get; set; }
        public int[] ID { get; set; }
        public string[] ColumnName { get; set; }
        public bool[] Used { get; set; }
        public bool[] Show { get; set; }
        public bool[] Search { get; set; }
        public string[] GroupName { get; set; }
    }
}
