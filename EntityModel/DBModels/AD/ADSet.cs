using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ADSet
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? Lang_ID { get; set; }
        public int? Site_ID { get; set; }
        public string Type_ID { get; set; }
        public int? Max_Num { get; set; }
        public bool? Show_AD { get; set; }
        public bool? Show_AD_Page { get; set; }
        public int? Max_Col { get; set; }
        public string SType { get; set; }
    }
}
