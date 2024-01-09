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
    public interface IModelPageEditManager
    {
         IEnumerable<ModelPageEditMain> GetAll();
         IEnumerable<ModelPageEditMain> Where(ModelPageEditMain model);
         Paging<ModelPageEditMain> Paging(SearchModelBase model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid,  string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        string CreatePageEdit(PageEditItemModel model, string LangId, string account);
        IList<SelectListItem> GetSelectList(string id);
        PageEditItemModel GetFirstModel(string id);
        string EditPageItem(PageEditItemModel model, string account);
        PageEditItemModel GetModelByID(string modelid, string itemid);
         Paging<PageIndexItem> PagingItem(SearchModelBase model);
        string UpdateStatus(string id, bool status, string account, string username);
        string AddItemUnit(string modelid, string name, string account);
        string UpdateItemUnit(string name, string id, string account);
        string UpdateItemSeq(int modelid, int id, int seq, string type, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string modelid, string account, string username);
        string SetUnitModel(PageUnitSettingModel model, string account);
        PageUnitSettingModel GetUnitModel(string modelid);
        IList<PageIndexItem> GetModelItem(string id);
        PageIndexItem GetlPageItem(string itemid);
    }
}
