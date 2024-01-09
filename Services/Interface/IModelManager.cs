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
    public interface IModelManager
    {
        PageIndexSettingModel GetPageIndexSettingModel(string lang_id);
        string SetPageIndexSettingModel(PageIndexSettingModel model, string langid,string account);
    }
}
