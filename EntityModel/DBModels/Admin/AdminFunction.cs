using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class AdminFunction
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? GroupID { get; set; }
        public string ItemName { get; set; }
        public string Parameter { get; set; }
        public int? ParentGroupID { get; set; }
    }
}
