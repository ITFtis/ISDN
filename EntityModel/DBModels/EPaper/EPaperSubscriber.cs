using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class EPaperSubscriber
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public string Account { get; set; }
        public string CHNName { get; set; }
        public string EMail { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DisableDate { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string OPDateStr { get; set; }
        
    }
}
