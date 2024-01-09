using Services;
using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;

namespace WebSiteProject
{
    public class InitConfig
    {
        public static void RegisterLang()
        {
            LangManager _lang = new LangManager(new ViewModels.DBModels.SQLRepositoryInstances(Common.connectionStr));
            CacheMapping.LangOption = _lang.GetLangOption();
        }
        public static void RegisterSiteData()
        {
            ISiteConfigManager _config = new SiteConfigManager(new ViewModels.DBModels.SQLRepositoryInstances(Common.connectionStr));
            var data = _config.GetSiteConfigModel();
            CacheMapping.PageTitle = data.Page_Title;
            CacheMapping.SEOScript = _config.GetSiteFlow().Siteflow_Code;
        }

        public static void RegisterLangText()
        {
            Common.SetAllLangKey();
        }
    }
}
