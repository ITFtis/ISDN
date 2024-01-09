using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ModelForumMain
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? ModelID { get; set; }
        public int? Lang_ID { get; set; }
        public string Name { get; set; }
        public string ForumDesc { get; set; }
        public int? Sort { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
