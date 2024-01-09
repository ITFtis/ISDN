using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ModPage
    {
        public int? id { get; set; }
        public int? lang_id { get; set; }
        public int? site_id { get; set; }
        public int? sort { get; set; }
        public string module_name { get; set; }
        public bool? enabled { get; set; }
        public DateTime? create_date { get; set; }
    }
}
