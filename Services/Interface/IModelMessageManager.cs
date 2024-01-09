using SQLModel.Models;

using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using ViewModels;

namespace Services.Interface
{
    public interface IModelMessageManager
    {
        IEnumerable<ModelMessageMain> GetAll();
        IEnumerable<ModelMessageMain> Where(ModelMessageMain model);
        Paging<ModelMessageMain> Paging(SearchModelBase model);
        Paging<MessageItemResult> PagingItem(string modelid, MessageSearchModel model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        IList<SelectListItem> GetGroupSelectList(string mainid);
        Paging<GroupMessage> PagingGroup(SearchModelBase model);
        string  EditGroup(string name, string id, string mainid, string account);
        string UpdateGroupSeq(int id, int seq, string mainid, string account, string username);
        string DeleteGroup(string[] idlist, string delaccount, string account, string username);
        string UpdateGroupStatus(string id, bool status, string account, string username);
        MessageUnitSettingModel GetUnitModel(string modelid);
        Paging<ColumnSetting> ColumnPaging(SearchModelBase model);
        string UpdateColumnStatus(string id, bool status, string account, string username);
        string UpdateColumnSeq(int id, int seq,  string account, string username);
        string SetUnitModel(MessageUnitSettingModel model, string account);
        SEOViewModel GetSEO(string modelid);
        string SaveSEO(SEOViewModel model, string LangID);
        string CreateItem(MessageEditModel model, string LangId, string account);
        MessageEditModel GetModelByID(string modelid, string itemid);
        string UpdateItem(MessageEditModel model, string LangId, string account);
        string UpdateItemSeq(int modelid, int id, int seq, string account, string username);
        string SetItemStatus(string id, bool status, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
        Paging<MessageItemResult> PagingItemForWebSite(string modelid, MessageSearchModel model,string nogroupstr);
        MessageItem GetModelItem(string itemid);
        string GetGroupName(string groupid);
        void UpdateClickCount(string itemid);
        SyndicationFeed GetSyndicationFeedData(string itemid, string menuid);
        IList<SelectListItem> GetAllGroupSelectList(string mainid);
        IList<SelectListItem> GetFrontGroupSelectList(string mainid);
    }
}
