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
    public interface IEPaperManager
    {
        string Create(EPaperEditModel model, string account, string langid);
        string Update(EPaperEditModel model, string account, string langid);
        EPaperEditModel GetModel(string id);
        Paging<EPaperItemResult> PagingItem(SearchModelBase model);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string SetIsEdit(string id, bool status, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        EPaperUnitSettingModel GetUnitModel(string langid);
        Paging<ColumnSetting> ColumnPaging(SearchModelBase model);
        string UpdateColumnStatus(string id, bool status, string account, string username);
        string UpdateColumnSeq(int id, int seq, string account, string username);
        string SetUnitModel(EPaperUnitSettingModel model, string account);
        Paging<EPaperAD> PagingDefaultADItem(SearchModelBase model);
        string EditADSeq(int id, int seq, string langid, string account, string username);
        string SetADDelete(string[] idlist, string delaccount, string langid, string account, string username);
        string EPaperDefaultADModel(EPaperDefaultADModel model, string account, string langid);
        EPaperDefaultADModel DefaultADModel(string id);
        string GetEPaperManuallyContent(string id);
        string SavePaperManuallyContent(string id, string content);
        Paging<EPaperContentItem> PagingMenuItem(SearchModelBase model);
        string SetEpaperItem(bool issel, string id, string itemid, string menuid, string modelid, string mainid);
        List<EPaperItemEdit> GetEPaperItemEdit(string id);
        string GetSortTable(string id);
        string EditItemSeq(string id, int seq, string type, string account, string username);
        string DeleteItems(string[] idlist);
        string SetItemStatus(string id, bool status, string account, string username);
        Paging<EPaperItemResult> PagingAdminItem(SearchModelBase model);
        string UpdateEpaperOrder(string id, string status);
        Paging<EPaperSubscriber> PagingEpaperOrder(SubscriberSearchModel model);
        string  AddSubscriber(string email, string account);
        string DelSubscriber(string[] idlist, string delaccount, string langid, string account, string username);
        byte[] GetExport(SubscriberSearchModel model);
        string SetSubscriberStatus(string id, bool status, string account, string username);
        string GetEPaperYearstr(string year = "");
        string SaveEPaperItemSort(Dictionary<string, string> allitemkey, string eid, string iseditsub);
        string DeleteEPaperItemSort(string[] delarrid, string eid);
        string DelSubscriber(string email, string delaccount);
        List<EPaperItem> GetEPaperItemList(string epaperid);
    }
}
