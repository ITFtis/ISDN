using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ClickCountTable
    {
        public string Type { get; set; }
        public string IP { get; set; }
        public string ID { get; set; }
        public DateTime? LastClickDatetime { get; set; }
    }
}
