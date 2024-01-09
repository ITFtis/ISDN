using SQLModel.Models;
using ViewModels;

namespace Services.Interface
{
    public interface IADManager
    {
         Paging<ADListResult> Paging(ADSearchModel model);
         ADSet GetADSet(string langid, string type, string stype);
         string SetMaxADCount(string langid, string type, string max,string stype);
        string UpdateSeq(int id, int seq, string type, string account, string username);
         string Delete(string[] idlist, string delaccount, string langid, string account, string username);
          string Create(ADEditModel mode, string account, string username);
         string Update(ADEditModel mode, string account, string username);
         ADEditModel GetModel(string id);
        string UpdateFixed(string id, bool status, string account, string username);
        string UpdateStatus(string id, bool status, string account, string username);
    }
}
