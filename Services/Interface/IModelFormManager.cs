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
    public interface IModelFormManager
    {
        IEnumerable<ModelFormMain> GetAll();
        IEnumerable<ModelFormMain> Where(ModelFormMain model);
        Paging<ModelFormMain> Paging(SearchModelBase model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        string EditSelItem(FormItemSettingModel model);
        FormItemSettingModel GetSelItemByID(string id);
        Paging<FormSelItem> PagingSelItem(SearchModelBase model);
        string SetItemIsMust(string id, bool status, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
        string UpdateItemSeq(int modelid, int id, int seq, string account, string username);
        SEOViewModel GetSEO(string mainid);
        string SaveSEO(SEOViewModel model, string LangID);
        FormUnitSettingModel GetUnitModel(string mainid);
        string SetUnitModel(FormUnitSettingModel model, string account);
        string SaveSetting(FormSettingModel model);
        FormSettingModel GetFormSetting(string mainid);
        string[] GetFormList(string itemid, IDictionary<string, string> langdict);
        string SaveForm(string jsonstr, string itemid);
        Paging<FormInputResult> PagingMail(MailSearchModel model);
        MailInputModel GetMailInput(string id);
        string SaveProgress(string progress, string id, string account);
        string SaveReply(string text, string id, string account);
        string SetMailDelete(string[] idlist, string delaccount, string account, string username);
        string SaveProgressNote(string text, string id, string account);
        byte[] GetExport(MailSearchModel model);
        string MainModelName(string mainid);
        List<FormSelItem> GetFormSelItemByItemID(string itemid);
        string[] GetFormListNoJs(string itemid, List<string> erroritem, IDictionary<string, string> langdict);
    }
}
