using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseJoinApply
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? ItemID { get; set; }
        public int? MainID { get; set; }
        public string SNCode { get; set; }
        public int PaymentStatus { get; set; }
        public DateTime? ActiveStDate { get; set; }
        public DateTime? ActiveEdDate { get; set; }
        public string Title { get; set; }
        public bool? CodeChk { get; set; }
        public string ActiveDesc { get; set; }
        public string Code { get; set; }
        public string SigninType { get; set; }
        public int Status { get; set; }
        public string Certificate { get; set; }
        public string ActiveHours { get; set; }
        public int GroupID { get; set; }
        public string SingUpEndDate { get; set; }
        
    }
}
