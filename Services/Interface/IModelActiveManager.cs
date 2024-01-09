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
    public interface IModelActiveManager
    {
        IEnumerable<ModelActiveEditMain> GetAll();
        IEnumerable<ModelActiveEditMain> Where(ModelActiveEditMain model);
        Paging<ModelActiveEditMain> Paging(SearchModelBase model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        IList<SelectListItem> GetActiveSelectList(string lang_id);
        string GetActiveItem(string modelid);
        IList<SelectListItem> GetGroupSelectList(string mainid);
        SEOViewModel GetSEO(string mainid);
        string SaveSEO(SEOViewModel model, string LangID);
        string UpdateGroupStatus(string id, bool status, string account, string username);
        string DeleteGroup(string[] idlist, string delaccount, string account, string username);
        string EditGroup(string name, string id, string mainid, string account);
        Paging<GroupActive> PagingGroup(SearchModelBase model);
        string UpdateGroupSeq(int id, int seq, string mainid, string account, string username);
        ActiveUnitSettingModel GetUnitModel(string mainid);
        Paging<ColumnSetting> ColumnPaging(SearchModelBase model);
        string UpdateColumnStatus(string id, bool status, string account, string username);
        string UpdateColumnSeq(int id, int seq, string account, string username);
        string SetUnitModel(ActiveUnitSettingModel model, string account);
        ActiveEditModel GetModelByID(string modelid, string itemid);
        string CreateItem(ActiveEditModel model, string LangId, string account);
        string UpdateItem(ActiveEditModel model, string LangId, string account);
        Paging<ActiveItemResult> PagingItem(string modelid, ActiveSearchModel model);
        string UpdateItemSeq(int modelid, int id, int seq, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
        string SetItemStatus(string id, bool status, string account, string username);
        string AddPhoto(AddPhotoModel model, string uploadfilepath, string user);
        Paging<ActivePhoto> PagingPhoto(SearchModelBase model);
        string UpdatePhotoSeq(int id, int seq, string itemid, string account, string username);
        string UpdatePhotoStatus(string id, bool status, string account, string username);
        string DeletePhoto(string[] idlist, string delaccount, string account, string username);
        string UpdatePhotoDesc(Dictionary<string, string> model);
        List<ActivePhoto> GetActiveItemList(string ItemID);
        int GetActivePhotoCount(string ItemID);
        ActiveItem GetModelItem(string itemid);
        string GetGroupName(string groupid);
        void UpdateClickCount(string itemid);
        Paging<ActiveItemResult> PagingItemForWebSite(string modelid, ActiveSearchModel model, string nogroupstr);
        IList<SelectListItem> GetAllGroupSelectList(string mainid);
        ActiveDateRange[] GetModelDataRange(string itemid);
        ActiveFiles GetActiveFiles(string fileid);
        ActiveFiles[] GetAllActiveFiles(string itemid);
        IList<SelectListItem> GetFrontGroupSelectList(string mainid);
    }
}
