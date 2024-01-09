using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ForumItem
    {
        [Key]
        [IsSequence]
        public int ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
    }
}
