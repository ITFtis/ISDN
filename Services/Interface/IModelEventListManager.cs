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
    public interface IModelEventListManager
    {
        IEnumerable<ModelEventListMain> GetAll();
        IEnumerable<ModelEventListMain> Where(ModelEventListMain model);
        Paging<ModelEventListMain> Paging(SearchModelBase model);
        Paging<EventListItemResult> PagingItem(string modelid, EventListSearchModel model);
        string AddUnit(string name, string langid, string account, ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        string UpdateItemSeq(int modelid, int id, int seq, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
        string SetItemStatus(string id, bool status, string account, string username);
        string CreateItem(EventListEditModel model, string LangId, string account);
        string UpdateItem(EventListEditModel model, string LangId, string account);
        EventListEditModel GetModelByID(string modelid, string itemid);
        IList<EventListItem> GetModelIDList(string modelid);
    }
}
