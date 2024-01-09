using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseEvent
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? ItemID { get; set; }
        public int? MainID { get; set; }
        public int? Sort { get; set; }
        public string EventName { get; set; }
    }
}


