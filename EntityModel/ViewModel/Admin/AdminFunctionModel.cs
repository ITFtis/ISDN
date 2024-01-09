using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AdminFunctionModel
    {
        public AdminFunctionModel()
        {
            AdminFixFunctionInput = new List<AdminFunctionAuth>();
            AdminMenuFunctionInput= new List<AdminFunctionAuth>();
            AdminFunctionList = new Dictionary<int, List<AdminFunction>>();
            MenuList = new List<Menu>();
            UsersList = new List<Users>();
            GroupName = "";
        }
        public List<AdminFunctionAuth> AdminFixFunctionInput { get; set; }
        public List<AdminFunctionAuth> AdminMenuFunctionInput { get; set; }
        public Dictionary<int, List<AdminFunction>> AdminFunctionList { get; set; }
        public IList<Menu> MenuList { get; set; }
        public IList<Users> UsersList { get; set; }
        public string GroupName { get; set; }
        public string GroupID { get; set; }
    }
}
