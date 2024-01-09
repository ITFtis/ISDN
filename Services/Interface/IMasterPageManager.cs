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
    public interface IMasterPageManager
    {
        void SetModel<T>(ref T model, string stype, string langid, string menuid) where T : MasterPageModel;
        MasterPageModel GetModel(string stype, string langid, string menuid);
        IList<ADBase> GetADMain(string stype, string langid);
        string GetSubString(string langid, string mid, IDictionary<string, string> menuurl = null);
        string GetAdminMenuString(string langid, string menutype, string groupid, string role, string openmenuid);
        string GetLinkString(string langid, string menutype, string groupid, string role, string openmenuid);
        List<HomePageLayoutModel> GetSiteLayout(List<PageLayout> sitemenu,  string title , string langid);
        string GetSubString(string sitemenuid);
        string GetFrontLinkString(string itemid, string mid, string itemname, string sitemenuid);
        string GetSiteMenuLinkString(string sitemenuid);
        string[] GetSEOData(string type, string typeid,string langid,string title="", bool leve2to3 = false);
        string[] GetSEOData(string type, string type2, string typeid, string typeid2, string langid,  string title = "");
        int GetMenuShowModel(string menuid);
        string CheckLangID(string mid);
        string CheckPagrAuth(bool EnterpriceStudentAuth, bool GeneralStudentAuth, bool VIPAuth, bool EMailAuth, string UserID, string connectionMemberStr, Dictionary<string, string> langdict);
        string GetLangName(string id);
        HomePageLayoutModel GetNewSiteLayout(string typename);
        string SetNoJS(int isnojs);
    }
}
