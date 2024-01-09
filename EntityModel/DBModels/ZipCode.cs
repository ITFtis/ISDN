using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ZipCode
    {
        public int CountryId { get; set; }
        public int Code { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
    }
}
