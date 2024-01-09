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
    public interface ISiteLayoutManager
    {
        string EditSiteLayout(SiteLayoutModel model);     
        string EditFowardSiteLayout(SiteLayoutModel model);
        string EditPrintSiteLayout(SiteLayoutModel model);
        string EditPage404SiteLayout(SiteLayoutModel model);
        SiteLayoutModel GetSiteLayout(string stype, string langid);
        Paging<PageLayout> PagingMain(SearchModelBase model);
        string Create(PageLayoutModel model, string langid, string account, string username);
        PageLayoutModel GetModel(string title, string stype, string langid);
        string Update(PageLayoutModel model, string account, string username);
        string UpdateSeq(int id, int seq, string langid, string type, string account, string username);
        string Delete(string[] idlist, string delaccount, string langid, string account, string username);
        SiteLangTextModel GetSiteLangText(string langid);
        string SaveSiteLangText(SiteLangTextModel model, string langid);
        string SavePageSeminar(PageSeminarModel model);
        PageSeminarModel GetPageSeminarModel();
        string SavePageNewEdit(PageNewsEditModel model);
        PageNewsEditModel GetPageNewEdit();
        string SavePageActiveEdit(PageActiveEditModel model);
        PageActiveEditModel GetPageActiveEdit();
        string SavePageJournalEdit(PageJournalEditModel model);
        PageJournalEditModel GetPageJournalEdit();
        void SaveActiveMoreInfo(string url);
        string GetActiveMoreInfo();
        void SaveNewsMoreInfo(string url);
        string GetNewsMoreInfo();
        string SetNoJS(int isnojs);
    }
}
