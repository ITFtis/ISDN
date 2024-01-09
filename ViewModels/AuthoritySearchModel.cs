using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AuthoritySearchModel : SearchModelBase
    {
        public AuthoritySearchModel()
        {
            Sort = "Account";
            GroupId = "";
            Name = "";
            Account = "";
            Status = "";
        }

        public string GroupId { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Status { get; set; }
        public string Readonly { get; set; }
    }
}
