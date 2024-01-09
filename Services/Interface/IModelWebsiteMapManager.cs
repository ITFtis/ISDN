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
    public interface IModelWebsiteMapManager
    {
         IEnumerable<ModelWebsiteMapMain> GetAll();
         IEnumerable<ModelWebsiteMapMain> Where(ModelWebsiteMapMain model);
         Paging<ModelWebsiteMapMain> Paging(SearchModelBase model);
        string AddUnit(string name, string langid, string account,ref int newid);
        string UpdateSeq(int id, int seq, string langid, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        string UpdateUnit(string name, string id, string account);
        SEOViewModel GetSEO(string mainid);
        string SaveUnit(SEOViewModel model, string LangID, Dictionary<string, string> Column);
        WebSiteEditModel GetModelByID(string mainid);
        string SaveInfo(WebSiteEditModel model, string account);
    }
}
