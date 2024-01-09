using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using Utilities;
using ViewModels;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace WebSiteProject.Areas.webadmin.Controllers
{

    public class EPaperController : AppController
    {
        IEPaperManager _IEPaperManager;
        IMenuManager _IMenuManager;
        public EPaperController()
        {
            _IMenuManager = serviceinstance.MenuManager;
            _IEPaperManager = serviceinstance.EPaperManager;
        }
        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.yeargroup = _IEPaperManager.GetEPaperYearstr();
            return View();
        }
        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult Subscriber()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View();
        }
        public ActionResult EPaperReview(string id)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = new EPaperEditModel();
            if (id != "-1")
            {
                model = _IEPaperManager.GetModel(id);
                model.EPaperItemEdit = _IEPaperManager.GetEPaperItemEdit(id);
            }
            return View(model);
        }

        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult EPaperManuallyContent(string id)
        {
            ViewBag.ID = id;
            ViewBag.HtmlContent = _IEPaperManager.GetEPaperManuallyContent(id);
            var itemdata = _IEPaperManager.GetModel(id);
            ViewBag.Title = itemdata.Title;
            return View();
        }

        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult EPaperItemSelect(string menuid, string id)
        {
            ViewBag.MenuId = menuid;
            ViewBag.ID = id;
            var menudata = _IMenuManager.GetModel(menuid);
            ViewBag.ModelID = menudata.ModelID;
            ViewBag.MainID = menudata.ModelItemID;
            var itemdata = _IEPaperManager.GetModel(id);
            ViewBag.Title = itemdata.Title;
            return View();
        }

        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult EPaperItemSort(string id)
        {
            ViewBag.ID = id;
            ViewBag.table = _IEPaperManager.GetSortTable(id);
            var model = _IEPaperManager.GetEPaperItemEdit(id);

            return View(model);
        }


        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult EPaperContentMenu(string id)
        {
            ViewBag.ID = id;
            var model = _IMenuManager.GetMenu(this.LanguageID, "1");
            var itemdata =  _IEPaperManager.GetModel(id);
            //過濾model
            var l3model = model.Where(v => v.MenuLevel == 3 && v.LinkMode == 2 && (v.ModelID == 2 || v.ModelID == 3  || v.ModelID == 7 || v.ModelID == 12));
            //var l3partnt = model.Where(v => l3model.Any(X => X.ParentID == v.ID));
            var l2model = model.Where(v => v.MenuLevel == 2 && (v.LinkMode == 2 && (v.ModelID == 2 || v.ModelID == 3  || v.ModelID == 7 || v.ModelID == 12))
            || (l3model.Any(X => X.ParentID == v.ID)));
            // var l2partnt = model.Where(v => l2model.Any(X => X.ParentID == v.ID));
            var l1model = model.Where(v => v.MenuLevel == 1 && (v.LinkMode == 2 && (v.ModelID == 2 || v.ModelID == 3  || v.ModelID == 7 || v.ModelID == 12))
            || (l2model.Any(X => X.ParentID == v.ID)));
            var allmember = l3model.Union(l2model).Union(l1model);
            ViewBag.Title = itemdata.Title;
            ViewBag.ItemList = _IEPaperManager.GetEPaperItemList(id);
            return View(allmember.ToList());
        }


        [AuthoridUrl("EPaper/Index", "")]
        #region EPaperEdit
        public ActionResult EPaperEdit(string id)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _IEPaperManager.GetModel(id);
            if (model.ID < 0)
            {
                ViewBag.Title = "新增電子報";
            }
            else {
                ViewBag.Title = model.Title;
            }
            return View(model);
        }
        #endregion

        #region AddADItem
        public ActionResult AddADItem(int Index)
        {
            ViewBag.index = Request["index"] == null ? "0" : Request["index"];
            ViewBag.seqindex = int.Parse(ViewBag.index) + 1;
            return PartialView();
        }
        #endregion

        #region SaveItem
        public ActionResult SaveItem(EPaperEditModel model)
        {
            if (model.TopBannerImg != null)
            {
                model.TopBannerImgOrgName = model.TopBannerImg.FileName.Split('\\').Last();
                var root = Request.PhysicalApplicationPath;
                var newpath = root + "\\UploadImage\\EPaper\\";
                if (System.IO.Directory.Exists(newpath) == false)
                {
                    System.IO.Directory.CreateDirectory(newpath);
                }
                var guid = Guid.NewGuid();
                var filename = DateTime.Now.Ticks + "." + model.TopBannerImg.FileName.Split('.').Last();
                var path = newpath + filename;
                model.TopBannerImgName = filename;
                model.TopBannerImgPath = path;
                model.TopBannerImg.SaveAs(path);
            }
            model.BottomHtmlContent = HttpUtility.UrlDecode(model.BottomHtmlContent);
            model.CenterHtmlContent = HttpUtility.UrlDecode(model.CenterHtmlContent);
            model.LeftHtmlContent = HttpUtility.UrlDecode(model.LeftHtmlContent);
            model.PageEndHtmlContent = HttpUtility.UrlDecode(model.PageEndHtmlContent);
            model.TopHtmlContent = HttpUtility.UrlDecode(model.TopHtmlContent);
            if (model.ID < 0)
            {
                Common.SetLogs(this.UserID, this.Account, "新增電子報項目=" + model.Title);
                return Json(_IEPaperManager.Create(model, Account, LanguageID));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改電子報項目ID=" + model.ID + " Name=" + model.Title);
                return Json(_IEPaperManager.Update(model, Account, LanguageID));
            }
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/EPaper/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/EPaper/" + filename);
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
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/EPaper/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/EPaper/" + filename);
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

        #region PagingItem
        public ActionResult PagingItem(SearchModelBase model)
        {
            model.LangId = LanguageID;
            return Json(_IEPaperManager.PagingAdminItem(model));
        }
        #endregion

        #region PagingEpaperOrder
        public ActionResult PagingEpaperOrder(SubscriberSearchModel model)
        {
            return Json(_IEPaperManager.PagingEpaperOrder(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更電子報管理單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IEPaperManager.UpdateSeq(id.Value, seq, "1", this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetIsEdit
        public ActionResult SetIsEdit(string id, bool status)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.SetIsEdit(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列電子報=" + delaccount);
                return Json(_IEPaperManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemStatus
        public ActionResult SetItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改電子報狀態ID=" + id + " status=" + status);
                return Json(_IEPaperManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        //====UnitSetting
        #region UnitSetting
        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult UnitSetting(string id)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _IEPaperManager.GetUnitModel(LanguageID);
            ViewBag.langid = LanguageID;
            return View(model);
        }
        #endregion

        #region PagingColumn
        public ActionResult PagingColumn(SearchModelBase model)
        {
            return Json(_IEPaperManager.ColumnPaging(model));
        }
        #endregion

        #region SetColumnStatus
        public ActionResult SetColumnStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.UpdateColumnStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditColumnSeq
        public ActionResult EditColumnSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.UpdateColumnSeq(id.Value, seq, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(EPaperUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定電子報單元管理");
                model.Summary = HttpUtility.UrlDecode(model.Summary);
                return Json(_IEPaperManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region DefaultAD
        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult DefaultAD()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());

            return View();
        }
        #endregion

        #region PagingDefaultADItem
        public ActionResult PagingDefaultADItem(SearchModelBase model)
        {
            model.LangId = LanguageID;
            return Json(_IEPaperManager.PagingDefaultADItem(model));
        }
        #endregion

        #region EditADSeq
        public ActionResult EditADSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.EditADSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetADDelete
        public ActionResult SetADDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.SetADDelete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region DefaultADEdit
        [AuthoridUrl("EPaper/Index", "")]
        public ActionResult DefaultADEdit(string id)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = new EPaperDefaultADModel();
            if (string.IsNullOrEmpty(id)) { return RedirectToAction("DefaultAD", "EPaper"); }
            if (id != "-1")
            {
                model = _IEPaperManager.DefaultADModel(id);
            }
            else
            {
                var _adsqlrepository = new SQLRepository<EPaperAD>(connectionstr);
                var adcount = _adsqlrepository.GetCountUseWhere("MainID=@1", new object[] { -1 });
                if (adcount >= 5)
                {
                    return RedirectToAction("DefaultAD", "EPaper");
                }
            }
            return View(model);
        }
        #endregion

        #region SaveDefaultEdit
        public ActionResult SaveDefaultEdit(EPaperDefaultADModel model)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.EPaperDefaultADModel(model, this.Account, this.LanguageID));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SaveManuallyContent
        public ActionResult SaveManuallyContent(string htmlcontent, string ID)
        {
            if (Request.IsAuthenticated)
            {
                htmlcontent = HttpUtility.UrlDecode(htmlcontent);
                return Json(_IEPaperManager.SavePaperManuallyContent(ID, htmlcontent));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region PagingMenuItem
        public ActionResult PagingMenuItem(SearchModelBase model)
        {
            model.LangId = LanguageID;
            return Json(_IEPaperManager.PagingMenuItem(model));
        }
        #endregion

        #region SetEpaperItem
        public ActionResult SetEpaperItem(bool issel, string itemid, string menuid, string id, string modelid, string mainid)
        {
            return Json(_IEPaperManager.SetEpaperItem(issel, id, itemid, menuid, modelid, mainid));
        }
        #endregion

        #region EditItemSeq
        public ActionResult EditItemSeq(string id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.EditItemSeq(id, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region DeleteEPaperItem
        public ActionResult DeleteEPaperItem(string[] list)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.DeleteItems(list));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region UpdateEpaperOrder
        public ActionResult UpdateEpaperOrder(string id, string status)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.UpdateEpaperOrder(id, status));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region UpdateEpaperOrder
        public ActionResult DisabledEpaperOrder(string id)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.UpdateEpaperOrder(id, "0"));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region AddSubscriber
        public ActionResult AddSubscriber(string sid, string name)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(name);
                }
                catch
                {
                    return Json("Email格式錯誤");
                }
                var str = "";
                if (sid == "-1" || string.IsNullOrEmpty(sid))
                {
                    Common.SetLogs(this.UserID, this.Account, "新增訂閱者=" + name);
                    str = _IEPaperManager.AddSubscriber(name,  this.Account);
                }
                //else
                //{
                //    Common.SetLogs(this.UserID, this.Account, "修改訊息管理單元名稱 ID=" + mainid + " 改為:" + name);
                //    str = _IMessageManager.UpdateUnit(name, mainid, this.Account);
                //}
                return Json(str);
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region DelSubscriber
        public ActionResult DelSubscriber(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IEPaperManager.DelSubscriber(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region Export
        public ActionResult Export(SubscriberSearchModel searchModel, string name)
        {
            try
            {
                if (name.IsNullorEmpty()) { name = "資料下載"; }
                string _fname = System.Web.HttpUtility.UrlEncode(name + ".xlsx", System.Text.Encoding.UTF8);
                Response.AddHeader("Content-Disposition", "attachment; filename='" + _fname + "';filename*=utf-8''" + _fname);
                return File(_IEPaperManager.GetExport(searchModel), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("匯出訂閱者管理列表失敗=" + ex.Message);
            }
            return Json("");
        }
        #endregion

        #region SetSubscriberStatus
        public ActionResult SetSubscriberStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改訊息管理狀態ID=" + id + " status=" + status);
                return Json(_IEPaperManager.SetSubscriberStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveEPaperItemSort
        public ActionResult SaveEPaperItemSort(Dictionary<string,string> allitemkey,string eid, string iseditsub)
        {
            string result = _IEPaperManager.SaveEPaperItemSort(allitemkey, eid, iseditsub);
            return Json(result);
        }
        #endregion

        #region DeleteEPaperItemSort
        public ActionResult DeleteEPaperItemSort(string[] delarrid, string eid)
        {
            string result = _IEPaperManager.DeleteEPaperItemSort(delarrid, eid);
            return Json(result);
        }
        #endregion
        
    }
}