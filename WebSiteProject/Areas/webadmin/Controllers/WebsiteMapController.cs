using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using Utilities;
using System.Configuration;
using System.IO;
using WebSiteProject.Code;

namespace WebSiteProject.Areas.webadmin.Controllers
{

    public class WebsiteMapController : AppController
    {
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        public WebsiteMapController()
        {
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            Session["IsFromClick"] = "Y";
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult WebSiteEdit(string mainid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            Session["IsFromClick"] = "Y";
            WebSiteEditModel model = _IModelWebsiteMapManager.GetModelByID(mainid);
            model.LangID = int.Parse(this.LanguageID);
            return View(model);
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
                    Common.SetLogs(this.UserID, this.Account, "新增網站導覽單元名稱=" + name);

                    var newid = 0;
                    str = _IModelWebsiteMapManager.AddUnit(name, this.LanguageID, this.Account, ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改網站導覽單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IModelWebsiteMapManager.UpdateUnit(name, mainid, this.Account);
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
            return Json(_IModelWebsiteMapManager.Paging(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更網站導覽單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IModelWebsiteMapManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列網站地圖配置=" + delaccount);
                return Json(_IModelWebsiteMapManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion


        #region ModelItem
        [AuthoridUrl("Model/Index", "")]
        public ActionResult ModelItem(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            Session["IsFromClick"] = "Y";
            ViewBag.mainid = mainid;

            var memuindex = Request["menuindex"];
            if (memuindex != null) { ViewBag.menustr = ""; }
            return View();
        }
        #endregion

        //==============
        #region UnitSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (Session["IsFromClick"] != null)
            {
                ViewBag.IsFromClick = "Y";
            }
            var maindata = _IModelWebsiteMapManager.Where(new ModelWebsiteMapMain() { ID = int.Parse(mainid) });
            var columnstr = maindata.First().ColumnDict;
            if (columnstr.IsNullorEmpty() == false)
            {
                var cdict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(columnstr);
                ViewBag.Column1 = cdict.ContainsKey("Column1") ? cdict["Column1"] : "";
                ViewBag.Column2 = cdict.ContainsKey("Column2") ? cdict["Column2"] : "";
                ViewBag.Column3 = cdict.ContainsKey("Column3") ? cdict["Column3"] : "";
            }

            return View(_IModelWebsiteMapManager.GetSEO(mainid));
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(SEOViewModel model, Dictionary<string, string> Column)
        {
            Common.SetLogs(this.UserID, this.Account, "設定網站導覽單元管理");
            return Json(_IModelWebsiteMapManager.SaveUnit(model, this.LanguageID, Column));
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/WebSiteMapItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/WebSiteMapItem/" + filename);
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

        #region UploadFile
        public ActionResult UploadFile(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            var filename = "";
            var imageUrl = "";
            if (upload != null && upload.ContentLength > 0)
            {
                //儲存圖片至Server
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/WebSiteMapItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/WebSiteMapItem/" + filename);
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

        #region AddSelItem
        public ActionResult AddSelItem(int Index)
        {
            ViewBag.index = Request["index"] == null ? "0" : Request["index"];
            ViewBag.seqindex = int.Parse(ViewBag.index) + 1;
            return PartialView();
        }
        #endregion
        #region SaveSelItem
        public ActionResult SaveInfo(WebSiteEditModel model)
        {
            model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
            model.LangID = int.Parse(this.LanguageID);
            string info = _IModelWebsiteMapManager.SaveInfo(model, this.Account);
            return Json(info);
        }
        #endregion
        
    }
}