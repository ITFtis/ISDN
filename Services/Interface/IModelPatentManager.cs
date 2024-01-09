using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModel;
using ViewModels;

namespace Services.Interface
{
    public interface IModelPatentManager
    {
        IEnumerable<ModelPatentMain> GetAll();
        IEnumerable<ModelPatentMain> Where(ModelPatentMain model);
        Paging<ModelPatentMain> Paging(SearchModelBase model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);

        IList<SelectListItem> GetGroupSelectList(string mainid, bool enabled);
        Paging<GroupPatent> PagingGroup(SearchModelBase model);
        string EditGroup(string name, string id, string mainid, string account);
        string DeleteGroup(string[] idlist, string delaccount, string account, string username);
        string UpdateGroupStatus(string id, bool status, string account, string username);
        string UpdateGroupSeq(int id, int seq, string mainid, string account, string username);
        IList<SelectListItem> GetAllGroupSelectList(string mainid);
        PatentEditModel GetModelByID(string modelid, string itemid);
        string CreateItem(PatentEditModel model, string languageID, string account);
        string UpdateItem(PatentEditModel model, string languageID, string account);
        Paging<PatentItemResult> PagingItem(string modelid, PatentSearchModel model);
        string UpdateItemSeq(int modelid, int id, int seq, string account, string username);
        string SetItemStatus(string id, bool status, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
        PatentUnitSettingModel GetUnitModel(string modelid);
        string SetUnitModel(PatentUnitSettingModel model, string account);
        PatentItem GetModelItem(string itemid);
        string GetGroupName(string groupid);
         IList<SelectListItem> GetGroupSelectList(string mainid);
        Paging<PatentItemResult> PagingItemForWebSite(string modelid, PatentSearchModel model, string nogroupstr);
    }
}
