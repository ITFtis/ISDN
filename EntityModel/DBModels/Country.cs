using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class Country
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}
