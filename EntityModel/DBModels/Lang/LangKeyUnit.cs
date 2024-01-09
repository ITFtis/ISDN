using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class LangKeyUnit
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public string  GroupName { get; set; }
        public int? SubGroup { get; set; }
        public string Item { get; set; }
        public int? Sort { get; set; }
        public int LKey { get; set; }
    }
}
