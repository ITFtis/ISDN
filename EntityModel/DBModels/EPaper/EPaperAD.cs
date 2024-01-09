using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class EPaperAD
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? MainID { get; set; }
        public string ADName { get; set; }
        public string ADLink { get; set; }
        public string ADFilePath { get; set; }
        public string ADFileName { get; set; }
        public int? ADIndex { get; set; }
        public int? LangID { get; set; }
    }

}
