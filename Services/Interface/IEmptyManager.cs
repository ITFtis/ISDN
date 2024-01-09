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
    public interface IEmptyManager
    {
         IEnumerable<Users> GetAll();
         IEnumerable<Users> Where(Users model);
         Paging<AdminMemberListResult> Paging(ADSearchModel model);
    }
}
