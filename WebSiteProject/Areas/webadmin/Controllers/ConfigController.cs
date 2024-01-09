using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using Utilities;
using ViewModels;

namespace WebSiteProject.Areas.webadmin.Controllers
{


    public class ConfigController : AppController
    {
        ISiteConfigManager _ISiteConfigManager;
        readonly SQLRepository<Users> _adminsqlrepository;
        readonly SQLRepository<SiteFlow> _siteflowsqlrepository;
        IAuthorityGroupManager _IAuthorityGroupManager;
        IAdminMemberManager _IAdminMemberManager;
        ILangManager _ILangManager;

        public ConfigController() {
            _ISiteConfigManager = serviceinstance.SiteConfigManager;
            _IAuthorityGroupManager = new AuthorityGroupManager(new SQLRepository<GroupUser>(connectionstr));
            _IAdminMemberManager = new AdminMemberManager(new SQLRepository<Users>(connectionstr));
            _ILangManager = serviceinstance.LangManager;
            _adminsqlrepository = new SQLRepository<Users>(connectionstr);
            _siteflowsqlrepository = new SQLRepository<SiteFlow>(connectionstr);
        }

        #region SiteConfig
        [AuthoridUrl("Config/SiteConfig","")]
        public ActionResult SiteConfig()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteConfigManager.GetAll();
            if (model.Count() > 0)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<SiteConfig, SiteConfigModel>());
                var mapper = config.CreateMapper();
                var cmodel = mapper.Map<SiteConfigModel>(model.First());
                cmodel.LogoFileUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/Config/" + cmodel.LogoUploadFileName);
                return View(cmodel);
            }
            else { return View(); }
        }
        #endregion

        #region SystemRecord
        [AuthoridUrl("Config/SystemRecord", "")]
        public ActionResult SystemRecord()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View();
        }
        #endregion

        #region SaveSiteConfig
        public ActionResult SaveSiteConfig(SiteConfigModel model)
        {
            try
            {
                var result = "";

                if (Request.Files.Count > 0)
                {
                    result = _ISiteConfigManager.Save(model, model.LogoUploadFile, "");
                }
                else
                {
                    result = _ISiteConfigManager.Save(model, null, "");
                    Code.CacheMapping.PageTitle = model.Page_Title;
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogError("UploadImageError:" + ex.Message);
                ViewData["Message"] = "作業失敗";
                return RedirectToAction("SiteConfig");
            }
        }
        #endregion

        #region User

        #region PasswordEdit
        [AuthoridUrl("Config/PasswordEdit", "")]
        public ActionResult PasswordEdit()
        {
            if (Request.IsAuthenticated)
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                var id = user.FindFirst("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider").Value;
                var oldaccount = _IAdminMemberManager.Where(new Users { ID =int.Parse(id) });
                ViewBag.account = oldaccount.First().Account;
                ViewBag.username = oldaccount.First().User_Name;
                return View();
            }
            else { return RedirectToAction("Login","Account", new { area = "webadmin" }); }
        }
        #endregion

        #region PasswordEdit
        public ActionResult SavePasswordEdit(string Password,string NewPassword,string ConfirmPassword)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                var id = user.FindFirst("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider").Value;
                var oldaccount = _IAdminMemberManager.Where(new Users { ID = int.Parse(id) });
                Password = Common.GetMD5(Password);
                NewPassword = Common.GetMD5(NewPassword);
                ConfirmPassword = Common.GetMD5(ConfirmPassword);
                if (Password != oldaccount.First().PWD){
                    return Json("原密碼輸入錯誤");
                }
                if (NewPassword != ConfirmPassword)
                {
                    return Json("新密碼確認與新密碼並不一致");
                }
                return Json(_IAdminMemberManager.UpdatePassword(id, NewPassword, account.Value, name));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region UserList
        [AuthoridUrl("Config/UserList", "")]
        public ActionResult UserList()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var list= _IAuthorityGroupManager.GetSelectList();
            list.First().Text = "全部";
            //ViewBag.grouplist = ViewBag.grouplist = list.Where(v => v.Value != "");  
            ViewBag.grouplist = list;
            return View();
        }
        #endregion

        #region UserListEdit
        [AuthoridUrl("Config/UserList", "")]
        public ActionResult UserListEdit(string ID="-1")
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var list= _IAuthorityGroupManager.GetSelectList();
            //ViewBag.grouplist = ViewBag.grouplist = list.Where(v => v.Value != "");  
            ViewBag.grouplist = list;
            var model=  _IAdminMemberManager.GetAdminMemberModelByID(int.Parse(ID));
            if (model.ID >= 0)
            {
                ViewBag.IsAdd = "N";
                ViewBag.Title = "編輯管理帳號";
            }
            else
            {
                ViewBag.IsAdd = "Y";
                ViewBag.Title = "新增管理帳號";
            }
            return View(model);
        }
        #endregion

        #region PagingUser
        #region Paging
        public ActionResult PagingUser(AuthoritySearchModel searchModel)
        {
            return Json(_IAdminMemberManager.Paging(searchModel));
        }
        #endregion

        #endregion

        #region SetStatus
        public ActionResult SetUserStatus(string id, bool status)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                return Json(_IAdminMemberManager.UpdateStatus(id, status, account.Value, name));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUserList
        public ActionResult SaveUserList(AdminMemberModel model)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                if (model.ID >= 0)
                {
                    var oldaccount = _IAdminMemberManager.Where(new Users { Account = model.Account });
                    if (oldaccount.Count() > 0) {
                        if (oldaccount.First().ID != model.ID) { return Json("修改的帳號已經存在"); }
                    }
                    if (_IAdminMemberManager.Update(model, account.Value, name)< 0){
                        return Json("儲存失敗");
                    }
                    return Json("");
                }
                else {
                    var oldaccount = _IAdminMemberManager.Where(new Users { Account = model.Account });
                    if (oldaccount.Count() > 0) { return Json("新增的帳號已經存在"); }
                    model.Password = Common.GetMD5(model.Password);
                    if ((_IAdminMemberManager.Create(model, account.Value, name) < 0)){
                        return Json("儲存失敗");
                    }
                    return Json("");
                }
               
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetUserDelete
        public ActionResult SetUserDelete(string[] idlist, string delaccount)
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
        #endregion

        #endregion

        #region Group

        #region SetGroupStatus
        public ActionResult SetGroupStatus(string id, bool status)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                Common.SetLogs(this.UserID, this.Account, "設定管理帳號群組id=" + id + "為" + status);
                return Json(_IAuthorityGroupManager.UpdateStatus(id, status, account.Value, name));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region GroupEdit
        [AuthoridUrl("Config/UserList", "")]
        public ActionResult GroupEdit()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View();
        }
        #endregion

        #region GroupAuth
        [AuthoridUrl("Config/UserList", "")]
        public ActionResult GroupAuth(string id,string langid,string groupname,string sellist, string oldlangid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (string.IsNullOrEmpty(langid)) { langid = LanguageID; }
            var model = _ISiteConfigManager.GetAdminFunctionModel(id, langid);
            if (groupname.IsNullorEmpty() == false) {
                model.GroupName = groupname;
            }
           ViewBag.langlist = _ILangManager.GetSelectList();
            ViewBag.groupid = model.GroupID ;
            ViewBag.langid = langid;
            if (oldlangid.IsNullorEmpty() == false)
            {
                if (Session["GroupAuth"] == null)
                {
                    var audist = new Dictionary<string, string>();
                    audist.Add(oldlangid, sellist);
                    Session["GroupAuth"] = audist;
                }
                else
                {
                    var audist = (Dictionary<string, string>)Session["GroupAuth"];
                    if (audist.ContainsKey(oldlangid))
                    {
                        audist[oldlangid] = sellist;
                    }
                    else
                    {
                        audist.Add(oldlangid, sellist);
                    }
                    Session["GroupAuth"] = audist;
                }
                var audist2 = (Dictionary<string, string>)Session["GroupAuth"];
                if (audist2.ContainsKey(langid))
                {
                    var list = audist2[langid].Split('^');
                    var fixlist = list.Where(v => v.IndexOf("fix") == 0).ToArray();
                    var menulist = list.Where(v => v.IndexOf("fix") != 0).ToArray();
                    model.AdminFixFunctionInput.Clear();
                    model.AdminMenuFunctionInput.Clear();
                    foreach (var fix in fixlist)
                    {
                        model.AdminFixFunctionInput.Add(new AdminFunctionAuth()
                        {
                            GroupID = int.Parse(model.GroupID),
                            LangID = int.Parse(langid),
                            Type = 0,
                            ItemID = int.Parse(fix.Replace("fix_", ""))
                        });
                    }
                    foreach (var menu in menulist)
                    {
                        model.AdminMenuFunctionInput.Add(new AdminFunctionAuth()
                        {
                            GroupID = int.Parse(model.GroupID),
                            LangID = int.Parse(langid),
                            Type = 0,
                            ItemID = int.Parse(menu.Replace("menu_", ""))
                        });
                    }
                }
            }
            else {
                Session["GroupAuth"] = null;
            }
            if (model.GroupName.IsNullorEmpty() == false) {
                ViewBag.Title = "群組權限管理";
            }
            else {
                ViewBag.Title = "新增群組";
            }
         
            return View(model);
        }
        #endregion

        #region GroupAuthSave
        public ActionResult GroupAuthSave(Dictionary<string,string> inputdata,string langid, string groupid,string groupname)
        {
            var oldauthlist = (Dictionary<string, string>)Session["GroupAuth"];
            _ISiteConfigManager.GroupAuthSave(langid, groupid, inputdata, groupname,Account, oldauthlist);
            return Json("");
        }
        #endregion

        #region PagingGroup
        public ActionResult PagingGroup(SearchModelBase searchModel)
        {
            return Json(_IAuthorityGroupManager.Paging(searchModel));
        }
        #endregion

        #region EditGroupSeq
        public ActionResult EditGroupSeq(int? groupid, int seq)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                Common.SetLogs(this.UserID, this.Account, "變更管理帳號群組排序GroupID=" + groupid  + "排序=" + seq);
                return Json(_IAuthorityGroupManager.UpdateSeq(groupid.Value,seq, account.Value, name));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetGroupDelete
        public ActionResult SetGroupDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除管理帳號群組=" + delaccount);
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                return Json(_IAuthorityGroupManager.Delete(idlist, delaccount, account.Value, name));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #endregion

        #region Lang
        #region SiteLang
        [AuthoridUrl("Config/SiteLang", "")]
        public ActionResult SiteLang()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View();
        }
        #endregion

        #region SiteLangEdit
        [AuthoridUrl("Config/SiteLang", "")]
        public ActionResult SiteLangEdit(string id)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = new SiteLangModel();
            ViewBag.langlist = _ILangManager.GetSelectList();
            if (id.IsNullorEmpty() == false) {
                model = _ILangManager.GetModelById(id);
            }
            var alldata = _ILangManager.GetAll().Where(v=>v.Deleted==false);
            if (id.IsNullorEmpty()) {
                id = "-1";
            }
            if (alldata.Any(v => v.ID == int.Parse(id)) == false) {
                if (alldata.Count() >= 3)
                {
                    ViewBag.disableadd = "Y";
                }
                else
                {
                    ViewBag.disableadd = "N";
                }
            }
           
            return View(model);
        }
        #endregion

        #region PagingLang
        public ActionResult PagingLang(SearchModelBase searchModel)
        {
            return Json(_ILangManager.Paging(searchModel));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更多國語系設定排序 ID=" + id + "排序=" + seq);
                return Json(_ILangManager.UpdateSeq(id.Value, seq,  this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetLangDelete
        public ActionResult SetLangDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                var str = _ILangManager.Delete(idlist, delaccount, account.Value, name);
                CacheMapping.LangOption = _ILangManager.GetLangOption();
                return Json(str);
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SaveLang
        public ActionResult SaveLang(SiteLangModel model)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                var alldata = _ILangManager.GetAll().Where(v => v.Deleted == false);
              
                if (model.ID > 0)
                {
                    alldata = alldata.Where(v => v.ID != model.ID);
                    if (alldata.Any(v => v.Lang_Name == model.Lang_Name || v.Sub_Domain_Name == model.Sub_Domain_Name))
                    {
                        return Json("語系、網址 不可以重複");
                    }
                    _ILangManager.Update(model, this.Account);
                }
                else {
                    if (alldata.Count() > 3)
                    {
                        return Json("語系無法再新增");
                    }
                    if (alldata.Any(v => v.Lang_Name == model.Lang_Name || v.Sub_Domain_Name==model.Sub_Domain_Name)){
                        return Json("語系、網址 不可以重複");
                    }
                    _ILangManager.Create(model, this.Account);
                }
                CacheMapping.LangOption = _ILangManager.GetLangOption();
                return Json("");
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetPublish
        public ActionResult SetPublish(string id)
        {
            if (Request.IsAuthenticated)
            {
                var str = _ILangManager.SetPublish(id);
                CacheMapping.LangOption = _ILangManager.GetLangOption();
                return Json(str);
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #endregion

        #region SiteFlow
        [AuthoridUrl("Config/SiteFlow", "")]
        public ActionResult SiteFlow()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var SiteFlow = _siteflowsqlrepository.GetAll();
            if (SiteFlow.Count() > 0)
            {
                var link = SiteFlow.First().Siteflow_Link;
                ViewBag.Code = SiteFlow.First().Siteflow_Code;
                ViewBag.Path = SiteFlow.First().Siteflow_Link;
                ViewBag.ID = SiteFlow.First().ID;
                if (link.IsNullorEmpty() == false) {
                    ViewBag.isClick = "Y";
                    ViewBag.SiteFlowURL = SiteFlow.First().Siteflow_Link;
                }
            }
            return View();
        }
        #endregion

        #region SiteFlowEdit
        [AuthoridUrl("Config/SiteFlow", "")]
        public ActionResult SiteFlowEdit()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var SiteFlow = _siteflowsqlrepository.GetAll();
            if (SiteFlow.Count() > 0) {
                ViewBag.Code = SiteFlow.First().Siteflow_Code;
                ViewBag.Path = SiteFlow.First().Siteflow_Link;
                ViewBag.ID= SiteFlow.First().ID;
            }
            return View();
        }
        #endregion

        #region GetSiteFlowFile
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetSiteFlowFile()
        {
            try
            {
                var root = Request.PhysicalApplicationPath;
                var filepath = root + "\\Example\\Google流量分析申請說明.pdf";  
                string filename = System.IO.Path.GetFileName(filepath);
                //讀成串流
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //回傳出檔案
                return File(iStream, "application/octet-stream", filename);
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("匯出系統權限管理失敗=" + ex.Message);
            }
            return Json("");
        }
        #endregion

        #region SetSiteFlowCode
        public ActionResult SetSiteFlowCode(string code,string path,string id)
        {
            var decode= HttpUtility.UrlDecode(code);
            var r = 0;
            if (id.IsNullorEmpty())
            {
                 r = _siteflowsqlrepository.Create(new SQLModel.Models.SiteFlow() {
                      Siteflow_Code= code == null ? "" : decode,
                       Siteflow_Link = path == null ? "" : path
                 });
            }
            else {
                 r = _siteflowsqlrepository.Update("Siteflow_Code=@1,Siteflow_Link=@2", "ID=@3", new object[] { code==null?"":decode,
            path ==null?"":path, id });
            }
        
            CacheMapping.SEOScript = decode;
            if (r > 0) { return Json(""); } else { return Json("更新失敗"); }
          
        }
        #endregion

        #region SiteConfig
        [AuthoridUrl("Config/MailServer", "")]
        public ActionResult MailServer()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteConfigManager.GetAll();
            if (model.Count() > 0)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<SiteConfig, SiteConfigModel>());
                var mapper = config.CreateMapper();
                var cmodel = mapper.Map<SiteConfigModel>(model.First());
                if (cmodel.Port <= 0) { cmodel.Port = 25; }
              
                return View(cmodel);
            }
            else { return View(); }
        }
        #endregion

        #region SaveMailServer
        public ActionResult SaveMailServer(SiteConfigModel model)
        {
            try
            {
                var result = "";
                if (Request.Files.Count > 0)
                {
                    result = _ISiteConfigManager.SaveMailServer(model,  "");
                }
                else
                {

                    result = _ISiteConfigManager.SaveMailServer(model, "");
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogError("UploadImageError:" + ex.Message);
                ViewData["Message"] = "作業失敗";
                return Json("作業失敗");
            }
        }
        #endregion

        #region PagingSystemRecord
        public ActionResult PagingSystemRecord(SystemRecordSearchModel searchModel)
        {
            return Json(_IAdminMemberManager.PagingSystemRecord(searchModel));
        }
        #endregion

        #region PagingSystemRecord
        public ActionResult LogDownload(string  id)
        {
            string log = _IAdminMemberManager.GetSystemRecordLog(id);
            var idarr = id.Split('_');
            var datetimesttr = new DateTime(long.Parse(idarr[0]));
            return File(Encoding.UTF8.GetBytes(log), "text/plain", "log_"+ datetimesttr.ToString("yyyyMMddHHmmss")+".txt");
        }
        #endregion

        #region Verify
        [AuthoridUrl("Config/Verify", "")]
        public ActionResult Verify()
        {
            if (Request.IsAuthenticated)
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
                return View();
            }
            else { return RedirectToAction("Login", "Account", new { area = "webadmin" }); }
        }
        #endregion

        #region PagingVerify
        public ActionResult PagingVerify(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            return Json(_ISiteConfigManager.PagingVerify(model));
        }
        #endregion

        #region SetVerifyOK
        public ActionResult SetVerifyOK(string id)
        {
            return Json(_ISiteConfigManager.SetVerifyOK(id, this.Account));
        }
        #endregion

        #region SetVerifyRefuse
        public ActionResult SetVerifyRefuse(string id)
        {
            return Json(_ISiteConfigManager.SetVerifyRefuse(id,this.Account));
        }
        #endregion

        #region Upload
        public ActionResult Upload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            var filename = "";
            var imageUrl = "";
            if (upload != null && upload.ContentLength > 0)
            {
                //儲存圖片至Server
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/Config/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/Config/" + filename);
                var vMessage = string.Empty;
                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            }
            return Json(new
            {
                uploaded = 1,
                fileName = filename,
                url = imageUrl
            });
            //return Content(result);

        }
        #endregion

        #region UploadFile
        public ActionResult UploadFile(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            var filename = "";
            var imageUrl = "";
            if (upload != null && upload.ContentLength > 0)
            {
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/Config/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/Config/" + filename);
                var vMessage = string.Empty;
                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            }
            return Json(new
            {
                uploaded = 1,
                fileName = filename,
                url = imageUrl
            });
        }
        #endregion
    }
}