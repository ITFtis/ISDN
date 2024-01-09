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
    public interface IModelFileDownloadManager
    {
        IEnumerable<ModelFileDownloadMain> GetAll();
        IEnumerable<ModelFileDownloadMain> Where(ModelFileDownloadMain model);
        Paging<ModelFileDownloadMain> Paging(SearchModelBase model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        IList<SelectListItem> GetGroupSelectList(string mainid,bool enabled);
        Paging<GroupFileDownload> PagingGroup(SearchModelBase model);
        string EditGroup(string name, string id, string mainid, string account);
        string DeleteGroup(string[] idlist, string delaccount, string account, string username);
        string UpdateGroupStatus(string id, bool status, string account, string username);
        string UpdateGroupSeq(int id, int seq, string mainid, string account, string username);
        SEOViewModel GetSEO(string mainid);
        string SaveSEO(SEOViewModel model, string LangID);
        FileDownloadUnitSettingModel GetUnitModel(string mainid);
        Paging<ColumnSetting> ColumnPaging(SearchModelBase model);
        string UpdateColumnStatus(string id, bool status, string account, string username);
        string UpdateColumnSeq(int id, int seq, string account, string username);
        string SetUnitModel(FileDownloadUnitSettingModel model, string account);
        FileDownloadEditModel GetModelByID(string modelid, string itemid);
        string CreateItem(FileDownloadEditModel model, string LangId, string account);
        string UpdateItem(FileDownloadEditModel model, string LangId, string account);
        Paging<FileDownloadItemResult> PagingItem(string modelid, FileDownloadSearchModel model);
        string UpdateItemSeq(int modelid, int id, int seq, string account, string username);
        string SetItemStatus(string id, bool status, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
        Paging<FileDownloadItemResult> PagingItemForWebSite(string modelid, FileDownloadSearchModel model,string  nogroupstr);
        FileDownloadItem GetModelItem(string itemid);
        void UpdateClickCount(string itemid);
        IList<SelectListItem> GetAllGroupSelectList(string mainid);
        FileDownloadFiles GetFileDownloadFiles(string fileid);
        List<FileDownloadFiles> GetAllFileDownloadFiles(string modelid);
        IList<SelectListItem> GetFrontGroupSelectList(string mainid);
    }
}
