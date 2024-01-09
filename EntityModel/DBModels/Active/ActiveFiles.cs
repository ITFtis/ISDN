using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ActiveFiles
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? ModelID { get; set; }
        public int? ItemID { get; set; }
        public string FileDesc { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int? FileIndex { get; set; }
        public int? LangID { get; set; }
    }

}
