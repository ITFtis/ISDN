using SQLModel.Models;

using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using ViewModels;

namespace Services.Interface
{
    public interface IModelArticleManager
    {
        IEnumerable<ModelArticleMain> GetAll();
        IEnumerable<ModelArticleMain> Where(ModelArticleMain model);
        Paging<ModelArticleMain> Paging(SearchModelBase model);
        Paging<ArticleItemResult> PagingItem(string modelid, ArticleSearchModel model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        IList<SelectListItem> GetGroupSelectList(string mainid);
        Paging<GroupArticle> PagingGroup(SearchModelBase model);
        string EditGroup(string name, string id, string mainid, string account, string uploadfilename, string uploadfileorgname, bool hassubgroup);
        string UpdateGroupSeq(int id, int seq, string mainid, string account, string username);
        string DeleteGroup(string[] idlist, string delaccount, string account, string username);
        string UpdateGroupStatus(string id, bool status, string account, string username);
        ArticleUnitSettingModel GetUnitModel(string modelid);
        Paging<ColumnSetting> ColumnPaging(SearchModelBase model);
        string UpdateColumnStatus(string id, bool status, string account, string username);
        string UpdateColumnSeq(int id, int seq,  string account, string username);
        string SetUnitModel(ArticleUnitSettingModel model, string account);
        SEOViewModel GetSEO(string modelid);
        string SaveSEO(SEOViewModel model, string LangID);
        string CreateItem(ArticleEditModel model, string LangId, string account);
        ArticleEditModel GetModelByID(string modelid, string itemid);
        string UpdateItem(ArticleEditModel model, string LangId, string account);
        string UpdateItemSeq(int modelid, int id, int seq, string account, string username);
        string SetItemStatus(string id, bool status, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
        Paging<ArticleItemResult> PagingItemForWebSite(string modelid, ArticleSearchModel model,string nogroupstr);
        ArticleItem GetModelItem(string itemid);
        string GetGroupName(string groupid);
        void UpdateClickCount(string itemid);
        SyndicationFeed GetSyndicationFeedData(string itemid, string menuid);
        IList<SelectListItem> GetAllGroupSelectList(string mainid);
        string[] GetArticleMore(string mainID, string nowid, string menuid);
        Paging<GroupItemArticle> PagingSubGroup(SearchModelBase model);
        string EditSubGroup(string name, string id, string mainid, string account, string uploadfilename, string uploadfileorgname, string groupid);
        string UpdateGroupSubSeq(int id, int seq, string mainid, string account, string username);
        string SetGroupSubDelete(string[] idlist, string delaccount, string account, string username);
        string UpdateGroupSubStatus(string id, bool status, string account, string username);
        List<GroupArticle> GetAllGroupList(string mainid);
        List<GroupItemArticle> GetAllGroupSubList(string mainid);
        List<ExtItemObj> GetExtItemObj(string mainid, string ExtItemList);
        List<string> GetNameList(string itemid);
        GroupArticle GetGroupByID(string groupid);
        List<GroupItemArticle> GetSubGroupList(string groupid);
        List<GroupItemArticle> GetSubGroupByID(string subgroupid);
    }
}
