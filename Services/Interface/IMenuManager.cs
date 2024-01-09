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
    public interface IMenuManager
    {
        string GetModelItem(string modelid, string langid, bool hasnewitem = true);
        string Create(MenuEditModel model, string account, string username);
        MenuEditModel GetModel(string id);
        string Update(MenuEditModel model, string account, string username);
        List<Menu> GetMenu(string languageID, string menutype);
        string DeleteMenu(string menuid);
        string Menudisabled(string menuid);
        string Menueabled(string menuid);
        string UpdateSort(int menuid, string type, string account, string username);
        string GetMenuOption(int langid, int level, int parentid, int type, int modelid=0);
        string[] GetMenuUrl(string id);
        Menu[] GetMenuTypeList(string type, string langid, string level);
        Menu[] GetMenuIDList(string menuid);
        string GetModelItemList(string menuid, string langid);
        string UpdateMenuLink(string linkurl, string menuid, string account, string userName);
    }
}
