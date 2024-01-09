using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using SQLModel;
using System.Web.Mvc;
using ViewModels;
using Utilities;
using System.Web;
using ViewModel;
using System.IO;

namespace Services.Manager
{
    public class AuthorityGroupManager : IAuthorityGroupManager
    {
        readonly SQLRepository<GroupUser> _sqlrepository;
        readonly SQLRepository<Users> _adminmembersqlrepository;
        public AuthorityGroupManager(SQLRepository<GroupUser> sqlrepository)
        {
            _sqlrepository = sqlrepository;
            _adminmembersqlrepository = new SQLRepository<Users>(sqlrepository._connectionString);
        }

        #region checkGroupName
        public bool checkGroupName(string groupname, int groupid)
        {
            var data = _sqlrepository.GetByWhere("Group_Name=@1", new object[] { groupname.Trim() });
            if (data.Count() > 0)
            {
                if (data.First().ID == groupid)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Create
        public int Create(int seq, string groupname, string account, string accountname)
        {
            return -1;
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var users = _adminmembersqlrepository.GetByWhere("Group_ID=@1",new object[] { idlist[idx] } );
                    if (users.Count() > 0) {
                        var gname=_sqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().Group_Name;
                        return "群組名稱:"+ gname+" 目前有所屬會員,無法刪除";
                    }
                    var entity = new GroupUser();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除管理群組失敗:ID=" + idlist[idx]);
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除管理群組:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _sqlrepository.GetAll().OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _sqlrepository.Update(tempmodel);
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除管理群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region GetAll
        public IEnumerable<GroupUser> GetAll()
        {
            return _sqlrepository.GetAll();
        }
        #endregion

        #region GetSelectList
        public IList<SelectListItem> GetSelectList()
        {
            var list = _sqlrepository.GetByWhere("Enabled=@1 Order By Sort", new object[] { true }, "ID, Group_Name");
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            item.Add(new System.Web.Mvc.SelectListItem() { Text = "請選擇", Value = "", Selected=true });
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Group_Name, Value = l.ID.ToString(),Selected=false });
            }
            return item;
        }
        #endregion

        #region Paging
        public Paging<UserGroupListResult> Paging(SearchModelBase model)
        {
            var Paging = new Paging<UserGroupListResult>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Group_Name like @1");
                    whereobj.Add("%" + model.Search + "%");
                }
            }

            var users = _adminmembersqlrepository.GetAll("Group_ID");
            var data = new List<GroupUser>();
            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                 data = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
                Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            }
            else
            {
                data = _sqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort);
                Paging.total = _sqlrepository.GetAllDataCount();
            }

            foreach (var d in data)
            {
                Paging.rows.Add(new UserGroupListResult()
                {
                    Enabled = d.Enabled,
                    Group_Name = d.Group_Name,
                    ID = d.ID,
                    Readonly = d.Readonly,
                    Seo_Manage = d.Seo_Manage,
                    Site_ID = d.Site_ID,
                    Sort = d.Sort,
                    UserCount = users.Count(v => v.Group_ID == d.ID)
                });
            }
           
            return Paging;
        }
        #endregion

        #region UpdateStatus
        public string UpdateStatus(string id, bool status, string account, string username)
        {
            try
            {
                //var entity = new GroupUser();
                //entity.Enabled = status ? true : false;
                //entity.UpdateDatetime = DateTime.Now;
                //entity.ID = int.Parse(id);
                //entity.UpdateUser = account;
                //var r = _sqlrepository.Update(entity);
                var r = _sqlrepository.Update("Enabled=@1,UpdateDatetime=@2,UpdateUser=@3", "ID=@4", new object[] { status, DateTime.Now, account, id });
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改管理群組顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改管理群組顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region CheckDelete
        public bool CheckDelete(string[] idlist)
        {
            var usegroup = _adminmembersqlrepository.GetAll();
            var allgroup = _sqlrepository.GetAll();
            if (usegroup == null) { return true; }
            var usegroupid = usegroup.GroupBy(v => v.Group_ID).Select(x => x.Key).ToArray();

            foreach (var id in idlist)
            {
                if (usegroupid.Contains(int.Parse(id)))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Update
        public int Update(int seq, string groupname, int groupid, string account, string accountname)
        {
            return 0;
        }
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<GroupUser> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 ", new object[] { seq }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetAllDataCount();
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<GroupUser> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1", new object[] { qseq }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        tempmodel.UpdateDatetime = updatetime;
                        tempmodel.UpdateUser = account;
                        _sqlrepository.Update(tempmodel);
                    }
                }
                oldmodel.First().Sort = seq;
                r = _sqlrepository.Update(oldmodel.First());
                if (r > 0)
                {
                    return "更新成功";
                }
                else {
                    return "更新失敗";
                }
             
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("更新群組排序失敗:" + " error:" + ex.Message);
                return "更新群組排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion
    }

}
