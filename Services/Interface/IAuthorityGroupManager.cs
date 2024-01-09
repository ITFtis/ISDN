using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewModel;
using ViewModels;

namespace Services.Interface
{
    public interface IAuthorityGroupManager
    {
         IEnumerable<GroupUser> GetAll();
         IList<System.Web.Mvc.SelectListItem> GetSelectList();
         Paging<UserGroupListResult> Paging(SearchModelBase model);
        bool checkGroupName(string groupname, int groupid);
        int Create(int seq, string groupname,string account, string accountname);
        int Update(int seq, string groupname,int groupid,string account, string accountname);   
        bool CheckDelete(string[] idlist);
        string Delete(string[] idlist, string delaccount, string account, string username);
        string UpdateStatus(string id, bool status, string account, string username);
        string UpdateSeq(int id, int seq, string account, string username);
    }
}
