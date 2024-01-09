using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ContactWindowData
    {
        public int ContactID { get; set; }
        public int MemberID { get; set; }
        public string Tel { get; set; }
        public string ExtTel { get; set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
