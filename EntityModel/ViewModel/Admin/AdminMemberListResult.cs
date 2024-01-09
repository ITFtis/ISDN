using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AdminMemberListResult
    {
        public string ID{ get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public string Email { get; set; }
        public string ExtTel { get; set; }
        public string ManagerIdList { get; set; }
        public bool Status { get; set; }
        public bool Readonly { get; set; }
    }
}
