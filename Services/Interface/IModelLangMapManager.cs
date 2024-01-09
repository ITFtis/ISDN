using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModel;
using ViewModels;

namespace Services.Interface
{
    public interface IModelLangMapManager
    {
         IEnumerable<ModelLangMain> GetAll();
         IEnumerable<ModelLangMain> Where(ModelLangMain model);
         Paging<ModelLangMain> Paging(ModelLangMain model);
        string AddUnit(string name, string langid, string account);
        string UpdateSeq(int id, int seq, string type, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
    }
}
