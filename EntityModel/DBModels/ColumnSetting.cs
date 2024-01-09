using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ColumnSetting
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public string Type { get; set; }
        public string ColumnName { get; set; }
        public int? ColumnKey { get; set; }
        public int? Sort { get; set; }
        public bool? Used { get; set; }
        public bool? Show { get; set; }
        public bool? Search { get; set; }
        public bool? IsMust { get; set; }
        public string InputType { get; set; }
        
    }
}
