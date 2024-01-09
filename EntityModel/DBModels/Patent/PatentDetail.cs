using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PatentDetail
    {
        public int? MainID { get; set; }
        public int? ItemID { get; set; }
        public int? GroupID { get; set; }
        public int? LangID { get; set; }
        public string Nation { get; set; }
        public string Patentno { get; set; }
        public string PatentDate { get; set; }
        public string EarlyPublicDate { get; set; }
        public string EarlyPublicNo { get; set; }
        public string Deadline { get; set; }
    }

}
