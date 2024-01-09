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
    public interface IAdminMemberManager
    {
         IEnumerable<Users> GetAll();
         IEnumerable<Users> Where(Users model);
         Paging<AdminMemberListResult> Paging(AuthoritySearchModel model);
         string GetManagerNameFromId(string id);
         int Create(AdminMemberModel entity,string account, string username);
         int Update(AdminMemberModel entity, string account, string username);
         bool checkAccount(string account,int id);
         string Delete(string[] idlist,string delaccount,string account, string username);
         string UpdateStatus(string id, bool status, string account, string username);
        AdminMemberModel GetAdminMemberModelByID(int ID);
        string UpdatePassword(string id, string password, string account, string username);
        Paging<SystemRecordResult> PagingSystemRecord(SystemRecordSearchModel searchModel);
        string GetSystemRecordLog(string id);
    }
}
