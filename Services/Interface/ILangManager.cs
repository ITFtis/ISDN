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
    public interface ILangManager
    {
        IEnumerable<Lang> GetAll();
        IList<System.Web.Mvc.SelectListItem> GetSelectList();
        Paging<Lang> Paging(SearchModelBase model);
        string GetLangOption();
        int Create(SiteLangModel model, string account);
        int Update(SiteLangModel model, string account);
        string Delete(string[] idlist, string delaccount, string account, string username);
        string UpdateSeq(int id, int seq, string account, string username);
        SiteLangModel GetModelById(string id);
        string SetPublish(string id);
        Paging<ModelLangMain> PagingMain(SearchModelBase model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateUnit(string name, string id, string account);
        string UpdateMainSeq(int id, int seq, string langid, string account, string username);
        string MainDelete(string[] idlist, string delaccount, string langid, string account, string username);
        ModelLangMain GetModelLangById(string id);
        void ChangeLangType(string mainid, string type, string langid);
    }
}
