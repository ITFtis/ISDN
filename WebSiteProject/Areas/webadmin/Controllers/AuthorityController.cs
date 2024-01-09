
using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using Utilities;
using ViewModel;
using ViewModels;
namespace WebSiteProject.Areas.webadmin.Controllers
{
    [Authorize]
    public class AuthorityController : AppController
    {
        // GET: Authority
        IAuthorityGroupManager _IAuthorityGroupManager;
        IAdminMemberManager _IAdminMemberManager;
        public AuthorityController()
        {
            _IAuthorityGroupManager = new AuthorityGroupManager(new SQLRepository<GroupUser>(connectionstr));
            _IAdminMemberManager = new AdminMemberManager(new SQLRepository<Users>(connectionstr));
        }
        // [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var AuthoritySearchModel = new AuthoritySearchModel();
            ViewBag.grouplist = _IAuthorityGroupManager.GetSelectList();
            return View(AuthoritySearchModel);
        }
        public ActionResult AuthorityGroup()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Import()
        {
            return View();
        }
        public ActionResult Edit(string ID, string selmanager, string selmember)
        {
            var grouplist = _IAuthorityGroupManager.GetSelectList();
            ViewBag.grouplist = grouplist;
            AdminMemberModel model = null;
          
            return View(model);
        }
        public ActionResult Save(AdminMemberModel model, string isAdd)
        {
            model.ID= int.Parse(model.EncryptID);
            if (model.GroupName == "總管理者" && model.ID<=0)
            {
                var admin = _IAdminMemberManager.Where(new Users()
                {
                    Group_ID = model.GroupId
                });
                if (admin.Count() > 0) { return Json("總管理者不可再新增"); }
            }
            if (_IAdminMemberManager.checkAccount(model.Account, model.ID) == false)
            {
                if (Request.IsAuthenticated)
                {
                    var user = Request.GetOwinContext().Authentication.User;
                    var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    var name = user.Identity.Name;
                    if (isAdd == "Y")
                    {                     
                        var r = _IAdminMemberManager.Create(model, account.Value, name);
                    }
                    else {
                        var r = _IAdminMemberManager.Update(model, account.Value, name);
                    }
                    return Json("");
                }
                else { return Json("請先登入"); }

            }
            else
            {
                return Json("帳號已經存在");
            }

        }

        #region EditGroupName
        public ActionResult EditGroupName(int seq, string groupname, int? groupid)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                if (_IAuthorityGroupManager.checkGroupName(groupname, groupid.Value))
                {
                    if (groupid >= 0)
                    {
                        var r = _IAuthorityGroupManager.Update(seq, groupname, groupid.Value, account.Value, name);
                        if (r > 0)
                        {
                            return Json("");
                        }
                        else { return Json("管理群組修改失敗"); }
                    }
                    else
                    {
                        var r = _IAuthorityGroupManager.Create(seq, groupname, account.Value, name);
                        if (r > 0)
                        {
                            return Json("管理群組新增成功");
                        }
                        else { return Json("管理群組新增失敗"); }
                    }
                }
                else
                {
                    return Json("管理群組已經存在");
                }
            }
            else { return Json("請先登入"); }
        } 
        #endregion

        #region Paging
        public ActionResult Paging(AuthoritySearchModel searchModel)
        {
            return Json(_IAdminMemberManager.Paging(searchModel));
        }
        #endregion
        
        public ActionResult PagingGroup(SearchModelBase searchModel)
        {
            return Json(_IAuthorityGroupManager.Paging(searchModel));
        }

        #region SetStatus
        public ActionResult SetStatus(string id, bool status)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                return Json("");
            }
            else { return Json("請先登入"); }
        } 
        #endregion

        public ActionResult SetGroupStatus(string id, bool status)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                Common.SetLogs(this.UserID, this.Account, "設定管理群組id=" + id + "為" + status);
                return Json(_IAuthorityGroupManager.UpdateStatus(id, status, account.Value, name));
            }
            else { return Json("請先登入"); }
        }

        public ActionResult SetDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                return Json(_IAdminMemberManager.Delete(idlist, delaccount, account.Value, name));
            }
            else { return Json("請先登入"); }

        }

        #region CheckDelete
        public ActionResult CheckDelete(string[] idlist)
        {
            if (_IAuthorityGroupManager.CheckDelete(idlist))
            {
                return Json("");
            }
            else
            {
                return Json("群組目前使用中,無法刪除");
            }

        }
        #endregion

        #region SetGroupDelete
        public ActionResult SetGroupDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                Common.SetLogs(this.UserID, this.Account, "刪除管理者群組=" + delaccount);
                return Json(_IAuthorityGroupManager.Delete(idlist, delaccount, account.Value, name));
            }
            else { return Json("請先登入"); }

        } 
        #endregion

        #region DownloadExcelTemplate
        [AllowAnonymous]
        public ActionResult DownloadExcelTemplate()
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string name = "管理權限列表匯入範例格式";

                string _fname = System.Web.HttpUtility.UrlEncode(name + ".xlsx", System.Text.Encoding.UTF8);
                Response.AddHeader("Content-Disposition", "attachment; filename='" + _fname + "';filename*=utf-8''" + _fname);
                var root = Request.PhysicalApplicationPath;
                var filepath = root + "\\Example\\AuthorityImport.xlsx";
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("DownloadExcelTemplate=" + ex.Message);
            }
            return Json("");
        } 
        #endregion

    }
}