using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ActivePhoto
    {
        [Key]
        [IsSequence]
        public int? PID { get; set; }
        public int? ItemID { get; set; }
        public int? MainID { get; set; }
        public int? Sort { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileDesc { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
