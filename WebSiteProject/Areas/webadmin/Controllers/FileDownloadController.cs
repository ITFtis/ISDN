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

    public class FileDownloadController : AppController
    {
        IModelFileDownloadManager _IModelFileDownloadManager;
        public FileDownloadController()
        {
            _IModelFileDownloadManager = serviceinstance.ModelFileDownloadManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.Title = "文件下載";
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
                    Common.SetLogs(this.UserID, this.Account, "新增文件下載管理單元名稱=" + name);
                    var newid = 0;
                    str = _IModelFileDownloadManager.AddUnit(name, this.LanguageID, this.Account, ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改文件下載單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IModelFileDownloadManager.UpdateUnit(name, mainid, this.Account);
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
            return Json(_IModelFileDownloadManager.Paging(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更文件下載單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IModelFileDownloadManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列文件下載=" + delaccount);
                return Json(_IModelFileDownloadManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region GroupEdit
        [AuthoridUrl("Model/Index", "")]
        public ActionResult GroupEdit(string mainid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = mainid;
            return View();
        }
        #endregion

        #region PagingGroup
        public ActionResult PagingGroup(SearchModelBase model)
        {
            return Json(_IModelFileDownloadManager.PagingGroup(model));
        }
        #endregion

        #region EditGroupSeq
        public ActionResult EditGroupSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更文件下載群組排序MainID=" + mainid + " ID =" + id + "排序=" + seq);
                return Json(_IModelFileDownloadManager.UpdateGroupSeq(id.Value, seq, mainid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetGroupDelete
        public ActionResult SetGroupDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除文件下載群組=" + delaccount);
                return Json(_IModelFileDownloadManager.DeleteGroup(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupStatus
        public ActionResult SetGroupStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定文件下載群組id=" + id + "為" + status);
                return Json(_IModelFileDownloadManager.UpdateGroupStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditGroup
        public ActionResult EditGroup(string name, string id, string mainid)
        {
            if (id == "-1" || id.IsNullorEmpty())
            {
                Common.SetLogs(this.UserID, this.Account, "新增文件下載群組名稱=" + name + " mainid=" + mainid);
                return Json(_IModelFileDownloadManager.EditGroup(name, id, mainid, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改文件下載群組名稱=" + name + " id=" + id);
                return Json(_IModelFileDownloadManager.EditGroup(name, id, mainid, this.Account));
            }

        }
        #endregion

        #region ModelItem
        [AuthoridUrl("Model/Index", "")]
        public ActionResult ModelItem(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (Session["IsFromClick"] != null)
            {
                ViewBag.IsFromClick = "Y";
            }

            var grouplist = _IModelFileDownloadManager.GetAllGroupSelectList(mainid);
            grouplist.Insert(0, new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            ViewBag.grouplist = grouplist;
            ViewBag.mainid = mainid;
            var maindata = _IModelFileDownloadManager.Where(new  ModelFileDownloadMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
            return View();
        }
        #endregion

        //==============
        #region SEOSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult SEOSetting(string mainid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View(_IModelFileDownloadManager.GetSEO(mainid));
        }
        #endregion

        #region SaveSEO
        public ActionResult SaveSEO(SEOViewModel model)
        {
            Common.SetLogs(this.UserID, this.Account, "修改文件下載SEO");
            return Json(_IModelFileDownloadManager.SaveSEO(model, this.LanguageID));
        }
        #endregion

        #region UnitSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = mainid;
            var model = _IModelFileDownloadManager.GetUnitModel(mainid);
            var maindata = _IModelFileDownloadManager.Where(new ModelFileDownloadMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
            return View(model);
        }
        #endregion

        #region PagingColumn
        public ActionResult PagingColumn(SearchModelBase model)
        {
            return Json(_IModelFileDownloadManager.ColumnPaging(model));
        }
        #endregion

        #region SetColumnStatus
        public ActionResult SetColumnStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelFileDownloadManager.UpdateColumnStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditColumnSeq
        public ActionResult EditColumnSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelFileDownloadManager.UpdateColumnSeq(id.Value, seq, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(FileDownloadUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定文件下載單元管理");
                return Json(_IModelFileDownloadManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion


        //===ActiveEdit
        #region ActiveEdit
        [AuthoridUrl("Model/Index", "")]
        public ActionResult FileDownloadEdit(string mainid, string itemid="-1")
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            ViewBag.isview = Request["isview"] == null ? "" : Request["isview"];
            if (ViewBag.isview == null || ViewBag.isview == "")
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
            var glist = _IModelFileDownloadManager.GetGroupSelectList(mainid, false);
            glist.First().Text = "無分類";
            ViewBag.grouplist = glist;
            FileDownloadEditModel model = null;
            model = _IModelFileDownloadManager.GetModelByID(mainid, itemid);

            return View(model);
        }
        #endregion

        #region SaveItem
        public ActionResult SaveItem(FileDownloadEditModel model)
        {
            model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
            if (model.RelateImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.RelateImageFileOrgName = model.RelateImageFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.RelateImageFileOrgName;
                var path = root + "\\UploadImage\\FileDownloadItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\FileDownloadItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\FileDownloadItem\\");
                }
                model.RelateImageFile.SaveAs(path);
                model.RelateImageName = newfilename;
            }


            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增文件下載=" + model.Title);
                return Json(_IModelFileDownloadManager.CreateItem(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改文件下載ID=" + model.ItemID + " Name=" + model.Title);
                return Json(_IModelFileDownloadManager.UpdateItem(model, this.LanguageID, this.Account));
            }
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(FileDownloadSearchModel model)
        {
            return Json(_IModelFileDownloadManager.PagingItem(model.ModelID.ToString(), model));
        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid, int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除文件下載排序ID=" + id + " sequence=" + seq);
                return Json(_IModelFileDownloadManager.UpdateItemSeq(modelid, id, seq, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemDelete
        public ActionResult SetItemDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改文件下載=" + delaccount);
                return Json(_IModelFileDownloadManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetItemStatus
        public ActionResult SetItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改文件下載狀態ID=" + id + " status=" + status);
                return Json(_IModelFileDownloadManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
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

        #region FileDownLoad
        public ActionResult FileDownLoad(string fileid)
        {
            var model = _IModelFileDownloadManager.GetFileDownloadFiles(fileid);
            string filepath ="";
            string oldfilename = "";
            if (model.FilePath.IsNullorEmpty() ==false)
            {
                //取得檔案名稱
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadFile";
                }
                filepath = uploadfilepath+ model.FilePath;
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                //讀成串流
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //回傳出檔案
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        #endregion

    }
}