using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using SQLModel.Models;
using SQLModel;
using ViewModels;
using Utilities;
using ViewModel;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HPSF;

namespace Services.Manager
{
    public class AdminMemberManager :IAdminMemberManager
    {
        readonly SQLRepository<Users> _sqlrepository;
        readonly SQLRepository<GroupUser> _authoritygrouprepository;
        readonly SQLRepository<SystemRecord> _systemrecordrepository;
        public AdminMemberManager(SQLRepository<Users> sqlrepository)
        {
            _sqlrepository = sqlrepository;
            _authoritygrouprepository = new SQLRepository<GroupUser>(sqlrepository._connectionString);
            _systemrecordrepository=new SQLRepository<SystemRecord>(sqlrepository._connectionString);
        }

        #region GetAll
        public IEnumerable<Users> GetAll()
        {
            return _sqlrepository.GetAll();
        } 
        #endregion

        #region Paging
        public Paging<AdminMemberListResult> Paging(AuthoritySearchModel model)
        {
            var Paging = new Paging<AdminMemberListResult>();
            var onepagecnt = model.Limit;
            List<Users> data = new List<Users>();
            var grouplist = _authoritygrouprepository.GetAll();
            if (model.Search == "")
            {
                Paging.total = _sqlrepository.GetAllDataCount();
                data = _sqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort);
            }
            else
            {
                var whereobj = new List<string>();
                var wherestr = new List<string>();
                var idx = 0;
                if (string.IsNullOrEmpty(model.GroupId) == false)
                {
                    idx += 1;
                    wherestr.Add("Group_ID=@" + idx);
                    whereobj.Add(model.GroupId);
                }
                if (string.IsNullOrEmpty(model.Name) == false)
                {
                    idx += 1;
                    wherestr.Add("User_Name like @" + idx);
                    whereobj.Add("%" + model.Name + "%");
                }
                if (string.IsNullOrEmpty(model.Account) == false)
                {
                    idx += 1;
                    wherestr.Add("Account like @" + idx);
                    whereobj.Add("%" + model.Account + "%");
                }
                if (string.IsNullOrEmpty(model.Status) == false)
                {
                    idx += 1;
                    wherestr.Add("Enabled=@" + idx);
                    whereobj.Add(model.Status);
                }

                if (wherestr.Count() > 0)
                {
                    var str = string.Join(" and ", wherestr);
                    data = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
                    Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());
                }
                else
                {
                    Paging.total = _sqlrepository.GetAllDataCount();
                    data = _sqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort);
                }
            }
            foreach (var d in data)
            {
                var td = grouplist.Where(v => v.ID == d.Group_ID);
                Paging.rows.Add(new AdminMemberListResult()
                {
                    Account = d.Account,
                    GroupId = d.Group_ID,
                    GroupName = td.Count() > 0 ? td.First().Group_Name : "",
                    ID = d.ID.ToString(),
                    Email=d.User_Email,
                    Name = d.User_Name,
                    Password = "",
                    Status = d.Enabled.Value,
                    Readonly=d.Readonly.Value
                });
            }
            return Paging;
        } 
        #endregion

        #region GetManagerNameFromId
        public string GetManagerNameFromId(string id)
        {
            var temparr = id.Split(',');
            System.Text.StringBuilder accountXML = new System.Text.StringBuilder();
            foreach (var item in temparr)
            {
                if (item != "") { accountXML.AppendFormat("<row><id>{0}</id></row>", item); }
            }
            var manager = _sqlrepository.GetByWhereIn(" EXISTS (select d.x.value('./id[1]','int') from @1.nodes('/*') as d(x) where Id=d.x.value('./id[1]','int'))", new object[] { accountXML.ToString() });
            if (manager.Count() > 0)
            {
                return manager.First().User_Name;
            }
            else { return ""; }
        } 
        #endregion

        #region Create
        public int Create(AdminMemberModel entity, string account, string username)
        {
            var r = -1;
            try
            {

                var oldmodel = new Users();
                oldmodel.PWD = entity.Password;
                oldmodel.Account = entity.Account;
                oldmodel.User_Name = entity.Name;
                oldmodel.Group_ID = entity.GroupId;
                oldmodel.Readonly = false;
                oldmodel.User_Email = "";
                oldmodel.Enabled = true;
                r = _sqlrepository.Create(oldmodel);
                if (r < 0)
                {
                    NLogManagement.SystemLogInfo("新增帳號內容失敗" + entity.Account);
                }
                NLogManagement.SystemLogInfo("新增帳號內容成功" + entity.Account);
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("新增管理會員失敗:" + ex.Message);
            }

            return r;
        } 
        #endregion

        #region checkAccount
        public bool checkAccount(string account, int id)
        {
            if (id != -1)
            {
                var lemgth = _sqlrepository.GetCountUseWhere("account=@1 and id!=@2", new object[] { account.Trim(), id });
                return lemgth > 0 ? true : false;
            }
            else
            {
                var lemgth = _sqlrepository.GetCountUseWhere("account=@1", new object[] { account.Trim() });
                return lemgth > 0 ? true : false;
            }
        } 
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var deleteaccount = "";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new Users();
                    entity.ID = int.Parse(idlist[idx]);
                    deleteaccount += entity.ID + ",";
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除管理會員失敗:ID=" + idlist[idx]);
                    }
                }
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除管理帳號:" + deleteaccount);
                    return "刪除成功";
                }
                else
                {
                    return "刪除失敗";
                }
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除管理會員失敗:" + ex.Message);
                return "刪除失敗";
            }
        } 
        #endregion

        #region Update
        public int Update(AdminMemberModel entity, string account, string username)
        {
            var r = -1;
            try
            {
                var oldmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { entity.ID }).First();
                oldmodel.PWD = entity.Password;
                oldmodel.Account =  entity.Account;
                oldmodel.User_Name = entity.Name;
                oldmodel.Group_ID = entity.GroupId;
             
                r = _sqlrepository.Update(oldmodel);
                if (r < 0) {
                    NLogManagement.SystemLogInfo("更新帳號內容失敗" + entity.Account);
                }
                NLogManagement.SystemLogInfo("更新帳號內容成功" + entity.Account);
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("新增管理會員失敗:" + ex.Message);
            }

            return r;
        }
        #endregion

        #region GetValue
        private string GetValue(NPOI.SS.UserModel.ISheet sheet, int rowidx, int columnidx)
        {
            var value = "";
            if (sheet.GetRow(rowidx) == null)
            {
                return "";
            }
            if (sheet.GetRow(rowidx).GetCell(columnidx) == null)
            {
                return "";
            }
            if (sheet.GetRow(rowidx).GetCell(columnidx).CellType == NPOI.SS.UserModel.CellType.Numeric)
            {
                value = sheet.GetRow(rowidx).GetCell(columnidx).NumericCellValue.ToString().Trim();
            }
            else if (sheet.GetRow(rowidx).GetCell(columnidx).CellType == NPOI.SS.UserModel.CellType.String)
            {
                value = sheet.GetRow(rowidx).GetCell(columnidx).StringCellValue.Trim();
            }
            else
            {
                return null;
            }
            return value;
        }
        #endregion

        #region SetValue
        private void SetValue(ISheet sheet, string value, int _r, int _c, ICellStyle style)
        {
            if (sheet.GetRow(_r) == null)
            {
                sheet.CreateRow(_r);
            }
            if (sheet.GetRow(_r).GetCell(_c) == null)
            {
                sheet.GetRow(_r).CreateCell(_c);
            }
            sheet.GetRow(_r).GetCell(_c).CellStyle = style;
            if (value == null) { value = ""; }
            sheet.GetRow(_r).GetCell(_c).SetCellValue(value);
        }

        #endregion

        #region Where
        public IEnumerable<Users> Where(Users model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region UpdateStatus
        public string UpdateStatus(string id, bool status, string account, string username)
        {
            try
            {
                //var entity = new Users();
                //entity.Enabled = status;
                //entity.UpdateDatetime = DateTime.Now;
                //entity.ID = int.Parse(id);
                //entity.UpdateUser = account;
                //var r = _sqlrepository.Update(entity);
                var r = _sqlrepository.Update("Enabled=@1,UpdateDatetime=@2,UpdateUser=@3", "ID=@4", new object[] { status, DateTime.Now, account, id });
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改帳號權限 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("新增管理會員狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region GetAdminMemberModelByID
        public AdminMemberModel GetAdminMemberModelByID(int ID)
        {
            var model = new AdminMemberModel();
            var AdminMember = _sqlrepository.GetByWhere("ID=@1", new object[] { ID });
            if (AdminMember != null && AdminMember.Count() > 0)
            {
                model.Account = AdminMember.First().Account;
                model.GroupId = AdminMember.First().Group_ID.Value;
                model.ID = AdminMember.First().ID.Value;
                model.Password = AdminMember.First().PWD;
                model.Name = AdminMember.First().User_Name;
            }
            return model;
        }
        #endregion

        #region UpdatePassword
        public string UpdatePassword(string id, string password, string account, string username)
        {
            try
            {
                var entity = new Users();
                entity.PWD = password;
                entity.UpdateDatetime = DateTime.Now;
                entity.ID = int.Parse(id);
                entity.UpdateUser = account;
                var r = _sqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改帳號密碼 id=" + id + " 為:" + password);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改帳號密碼失敗:" + ex.Message);
                return "更新失敗";
            }
        }

        #endregion

        #region PagingSystemRecord
        public Paging<SystemRecordResult> PagingSystemRecord(SystemRecordSearchModel model)
        {
            var Paging = new Paging<SystemRecordResult>();
            List<SystemRecord> data = new List<SystemRecord>();
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            var idx = 0;
            if (string.IsNullOrEmpty(model.IP) == false)
            {
                idx += 1;
                wherestr.Add("IP like @" + idx);
                whereobj.Add("%" + model.IP + "%");
            }
            if (string.IsNullOrEmpty(model.Account) == false)
            {
                idx += 1;
                wherestr.Add("Account like @" + idx);
                whereobj.Add("%" + model.Account + "%");
            }
            if (string.IsNullOrEmpty(model.LoginDateFrom) == false)
            {
                idx += 1;
                wherestr.Add("(Login >=@" + idx + " or Login=Null)");
                whereobj.Add(model.LoginDateFrom);
            }
            if (string.IsNullOrEmpty(model.LoginDateTo) == false)
            {
                idx += 1;
                wherestr.Add("(Login <=@" + idx + " or  Login=Null)");
                whereobj.Add(model.LoginDateTo);
            }

            if (string.IsNullOrEmpty(model.LogoutDateFrom) == false)
            {
                idx += 1;
                wherestr.Add("(Logout >=@" + idx + " or Logout=Null)");
                whereobj.Add(model.LogoutDateFrom);
            }
            if (string.IsNullOrEmpty(model.LogoutDateTo) == false)
            {
                idx += 1;
                wherestr.Add("(Logout <=@" + idx + " or  Logout=Null)");
                whereobj.Add(model.LogoutDateTo);
            }


            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                data = _systemrecordrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
                Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            }
            else
            {
                Paging.total = _systemrecordrepository.GetAllDataCount();
                data = _systemrecordrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort);
            }

            foreach (var d in data)
            {
                Paging.rows.Add(new SystemRecordResult()
                {
                    Account = d.Account,
                    IP = d.IP,
                    UserID = d.UserID,
                    Login = d.Login == null ? "" : d.Login.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    Logout = d.Logout == null ? "" : d.Login.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    TempID = d.Login.Value.Ticks.ToString() + "_" + d.UserID
                });
            }

            return Paging;
        }


        #endregion
        #region GetSystemRecordLog
        public string GetSystemRecordLog(string id)
        {
            var idarr = id.Split('_');
            var datetimesttr = new DateTime(long.Parse(idarr[0]));
            var data = _systemrecordrepository.GetByWhere("Login=@1 and UserID=@2", new object[] { datetimesttr, idarr[1] });
            if (data.Count() > 0)
            {
                return data.First().Logs;
            }
            else {
                return "";
            }
        }
       #endregion
    }
}
