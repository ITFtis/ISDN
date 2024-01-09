using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class UserGroupListResult
    {
        public int? ID { get; set; }
        public int? Site_ID { get; set; }
        public int? Sort { get; set; }
        public int UserCount { get; set; }
        public string Group_Name { get; set; }
        public bool? Seo_Manage { get; set; }
        public bool Readonly { get; set; }
        public bool Enabled { get; set; }
    }
}
