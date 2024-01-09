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
    public interface ISiteConfigManager
    {
        IEnumerable<SiteConfig> GetAll();
        IEnumerable<SiteConfig> Where(SiteConfig model);
        string Save(SiteConfigModel model, System.Web.HttpPostedFileBase uploadfile, string updateuser);
        AdminFunctionModel GetAdminFunctionModel(string groupid, string langid);
        void GroupAuthSave(string languageID, string groupid, Dictionary<string, string> inputdata, string groupname, string account, Dictionary<string, string> oldlist);
        SiteConfigModel GetSiteConfigModel();
        SiteFlow GetSiteFlow();
        string SaveMailServer(SiteConfigModel model, string updateuser);
        string SaveQuestionnaireFinishDesc(int ID, string QuestionnaireFinishDesc, string updateuser);
        Paging<VerifyDataResult> PagingVerify(SearchModelBase model);
        string SetVerifyOK(string id,string account);
        string SetVerifyRefuse(string id,string account);
    }
}
