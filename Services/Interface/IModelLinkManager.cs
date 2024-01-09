using SQLModel.Models;

using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using ViewModels;

namespace Services.Interface
{
    public interface IModelLinkManager
    {
        Paging<LinkItemResult> PagingItem(string isfront, SearchModelBase model);
        string CreateItem(LinkEditModel model, string LangId, string account);
        LinkEditModel GetModelByID(string modelid, string itemid);
        string UpdateItem(LinkEditModel model, string LangId, string account);
        string UpdateItemSeq(int langid, int id, int seq, string account, string username,string type);
        string SetItemStatus(string id, bool status, string account, string username);
        string DeleteItem(string[] idlist, string delaccount, string account, string username);
    }
}
