
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
    public class FormController : AppController
    {
        IModelFormManager _IModelFormManager;
        public FormController()
        {
            _IModelFormManager = serviceinstance.ModelFormManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.Title = "表單管理";
            Session["IsFromClick"] = "Y";
            return View();
        }

        //====Main
        #region EditUnit
        public ActionResult EditUnit(string mainid, string name)
        {
            if (Request.IsAuthenticated)
            {
                var str = "";
                if (mainid == "-1")
                {
                    var newid = 0; Common.SetLogs(this.UserID, this.Account, "新增表單管理單元名稱=" + name);
                    str = _IModelFormManager.AddUnit(name, this.LanguageID, this.Account, ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改表單管理單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IModelFormManager.UpdateUnit(name, mainid, this.Account);
                }
                return Json(str);
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region PagingMain
        public ActionResult PagingMain(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            return Json(_IModelFormManager.Paging(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更表單管理單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IModelFormManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列表單管理=" + delaccount);
                return Json(_IModelFormManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region MailModelItem
        [AuthoridUrl("Model/Index", "")]
        public ActionResult MailModelItem(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (Session["IsFromClick"] != null)
            {
                ViewBag.IsFromClick = "Y";
            }
            ViewBag.mainid = mainid;
            ViewBag.mainname = _IModelFormManager.MainModelName(mainid);
            return View();
        }
        #endregion

        #region PagingMain
        public ActionResult PagingMailItem(MailSearchModel model)
        {
            model.LangId = this.LanguageID;
            return Json(_IModelFormManager.PagingMail(model));
        }
        #endregion

        //===============
        #region FormManager
        [AuthoridUrl("Model/Index", "")]
        public ActionResult FormManager(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            ViewBag.isview = Request["isview"] == null ? "" : Request["isview"];
            if (ViewBag.isview == null || ViewBag.isview == "")
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
            if (Session["IsFromClick"] != null)
            {
                ViewBag.IsFromClick = "Y";
            }
            ViewBag.mainid = mainid;
            ViewBag.mainname = _IModelFormManager.MainModelName(mainid);
            return View();
        }
        #endregion

        #region PagingSelItem
        public ActionResult PagingSelItem(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            return Json(_IModelFormManager.PagingSelItem(model));
        }
        #endregion

        //===============
        #region ItemEdit
        [AuthoridUrl("Model/Index", "")]
        public ActionResult ItemEdit(string mainid, string id)
        {
            FormItemSettingModel model = null;
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (id.IsNullorEmpty() == false)
            {
                model = _IModelFormManager.GetSelItemByID(id);
            }
            else
            {
                model = new FormItemSettingModel();
                model.MainID = int.Parse(mainid);
            }
            ViewBag.mainname = _IModelFormManager.MainModelName(mainid);
            return View(model);
        }
        #endregion

        #region AddSelItem
        public ActionResult AddSelItem(int Index)
        {
            ViewBag.index = Request["index"] == null ? "0" : Request["index"];
            ViewBag.seqindex = int.Parse(ViewBag.index) + 1;
            return PartialView();
        }
        #endregion

        #region SaveSelItem
        public ActionResult SaveSelItem(FormItemSettingModel model)
        {

            return Json(_IModelFormManager.EditSelItem(model));
        }
        #endregion

        #region SetItemIsMust
        public ActionResult SetItemIsMust(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除項目必填id=" + id + " Status" + status);
                return Json(_IModelFormManager.SetItemIsMust(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemDelete
        public ActionResult SetItemDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelFormManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid, int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改表單管理排序ID=" + id + " sequence=" + seq);
                return Json(_IModelFormManager.UpdateItemSeq(modelid, id, seq, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        //==============
        #region SEOSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult SEOSetting(string mainid)
        {
            ViewBag.mainid = mainid;
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View(_IModelFormManager.GetSEO(mainid));
        }
        #endregion

        #region SaveSEO
        public ActionResult SaveSEO(SEOViewModel model)
        {
            Common.SetLogs(this.UserID, this.Account, "修改表單管理SEO");
            return Json(_IModelFormManager.SaveSEO(model, this.LanguageID));
        }
        #endregion

        //====UnitSetting
        #region UnitSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = mainid;
            var model = _IModelFormManager.GetUnitModel(mainid);
            ViewBag.mainname = _IModelFormManager.MainModelName(mainid);
            return View(model);
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(FormUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定表單管理單元管理");
                return Json(_IModelFormManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        //============
        #region FormSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult FormSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = mainid;
            var model = _IModelFormManager.GetFormSetting(mainid);
            ViewBag.mainname = _IModelFormManager.MainModelName(mainid);
            return View(model);
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
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/FormItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/FormItem/" + filename);
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

        #region SaveSetting
        public ActionResult SaveSetting(FormSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                model.FormDesc = HttpUtility.UrlDecode(model.FormDesc);
                model.ConfirmContent = HttpUtility.UrlDecode(model.ConfirmContent);
                return Json(_IModelFormManager.SaveSetting(model));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region MailProcess
        [AuthoridUrl("Model/Index", "")]
        public ActionResult MailProcess(string itemid)
        {
            if (itemid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _IModelFormManager.GetMailInput(itemid);
            model.MainID = model.MainID;
            ViewBag.Title= _IModelFormManager.MainModelName(model.MainID.ToString());
            NLogManagement.SystemLogInfo("Into MailProcess....");
            return View(model);
        }
        #endregion

        #region SaveProgress
        public ActionResult SaveProgressNote(string text, string id)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelFormManager.SaveProgressNote(text, id, Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveReply
        public ActionResult SaveReply(string text, string progress, string id)
        {
            NLogManagement.SystemLogInfo("Into SaveReply....");
            if (Request.IsAuthenticated)
            {
                return Json(_IModelFormManager.SaveReply(text, id, Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMailDelete
        public ActionResult SetMailDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelFormManager.SetMailDelete(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SaveProgress
        public ActionResult SaveProgress(string progress, string id)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelFormManager.SaveProgress(progress, id, Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region Export
        public ActionResult Export(MailSearchModel searchModel, string fname)
        {
            try
            {
                if (fname.IsNullorEmpty()) { fname = "資料下載"; }
                string _fname = System.Web.HttpUtility.UrlEncode(fname + ".xlsx", System.Text.Encoding.UTF8);
                Response.AddHeader("Content-Disposition", "attachment; filename='" + _fname + "';filename*=utf-8''" + _fname);
                return File(_IModelFormManager.GetExport(searchModel), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("匯出數據資料庫列表失敗=" + ex.Message);
            }
            return Json("");
        }
        #endregion
    }
}