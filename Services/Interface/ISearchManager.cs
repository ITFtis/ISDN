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
    public interface ISearchManager
    {
        PagingInfo<SearchResult> Paging(AdvanceSearchModel model);
        void SetKeyCount(string key, string langid);
    }
}
