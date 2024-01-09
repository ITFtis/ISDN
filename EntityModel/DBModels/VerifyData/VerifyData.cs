using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class VerifyData
    {
        public int ModelID { get; set; }
        public int ModelItemID { get; set; }
        public int ModelMainID { get; set; }
        public string ModelName { get; set; }
        public int VerifyStatus { get; set; }
        [EmptyNull]
        public DateTime? VerifyDateTime { get; set; }
        public string VerifyUser { get; set; }
        public string VerifyName { get; set; }
        public int ModelStatus { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateAccount { get; set; }
       [EmptyNull]
        public DateTime? UpdateDateTime { get; set; }
        public int LangID { get; set; }
    }
    
}
