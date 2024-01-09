using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class LogisticsCompany
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? Sort { get; set; }
        public string SendType { get; set; }
        public string Company { get; set; }
        public string SearchWebsite { get; set; }
    }
}
