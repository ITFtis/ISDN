using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class SystemRecord
    {
        public int UserID { get; set; }
        public string Account { get; set; }
        public string IP { get; set; }
        public DateTime? Login { get; set; }
        public DateTime? Logout { get; set; }
        public string Logs { get; set; }
    }
}
