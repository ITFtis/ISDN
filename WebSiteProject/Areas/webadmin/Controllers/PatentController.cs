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

    public class PatentController : AppController
    {
        IModelPatentManager _IModelPatentManager;
        public PatentController()
        {
            _IModelPatentManager = serviceinstance.ModelPatentManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.Title = "專利管理";
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
                    Common.SetLogs(this.UserID, this.Account, "新增專利管理管理單元名稱=" + name);
                    var newid = 0;
                    str = _IModelPatentManager.AddUnit(name, this.LanguageID, this.Account, ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改專利管理單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IModelPatentManager.UpdateUnit(name, mainid, this.Account);
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
            return Json(_IModelPatentManager.Paging(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更專利管理單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IModelPatentManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列專利管理=" + delaccount);
                return Json(_IModelPatentManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        //===============
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
            var grouplist = _IModelPatentManager.GetAllGroupSelectList(mainid);
            grouplist.Insert(0, new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            ViewBag.grouplist = grouplist;
            ViewBag.mainid = mainid;
            var maindata = _IModelPatentManager.Where(new  ModelPatentMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
            return View();
        }
        #endregion

        //=====Group
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
            return Json(_IModelPatentManager.PagingGroup(model));
        }
        #endregion

        #region EditGroupSeq
        public ActionResult EditGroupSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更專利管理排序MainID=" + mainid + " ID =" + id + "排序=" + seq);
                return Json(_IModelPatentManager.UpdateGroupSeq(id.Value, seq, mainid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetGroupDelete
        public ActionResult SetGroupDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除專利管理=" + delaccount);
                return Json(_IModelPatentManager.DeleteGroup(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupStatus
        public ActionResult SetGroupStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定專利管理d=" + id + "為" + status);
                return Json(_IModelPatentManager.UpdateGroupStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditGroup
        public ActionResult EditGroup(string name, string id, string mainid)
        {
            if (id == "-1" || id.IsNullorEmpty())
            {
                Common.SetLogs(this.UserID, this.Account, "新增專利管理名稱=" + name + " mainid=" + mainid);
                return Json(_IModelPatentManager.EditGroup(name, id, mainid, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改專利管理名稱=" + name + " id=" + id);
                return Json(_IModelPatentManager.EditGroup(name, id, mainid, this.Account));
            }

        }
        #endregion

        //===============

        #region PatentEdit
        [AuthoridUrl("Model/Index", "")]
        public ActionResult PatentEdit(string mainid, string itemid="-1")
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            ViewBag.isview = Request["isview"] == null ? "" : Request["isview"];
            if (ViewBag.isview == null || ViewBag.isview == "")
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
            ViewBag.grouplist = _IModelPatentManager.GetAllGroupSelectList(mainid);
            PatentEditModel model = null;
            model = _IModelPatentManager.GetModelByID(mainid, itemid);
            return View(model);
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(PatentSearchModel model)
        {
            return Json(_IModelPatentManager.PagingItem(model.ModelID.ToString(), model));
        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid, int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改專利管理排序ID=" + id + " sequence=" + seq);
                return Json(_IModelPatentManager.UpdateItemSeq(modelid, id, seq, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemStatus
        public ActionResult SetItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改專利管理狀態ID=" + id + " status=" + status);
                return Json(_IModelPatentManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemDelete
        public ActionResult SetItemDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除訊息管理=" + delaccount);
                return Json(_IModelPatentManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
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

        #region SaveItem
        public ActionResult SaveItem(PatentEditModel model)
        {
            if (model.UploadFile != null)
            {
                model.UploadFileName = model.UploadFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newpath = uploadfilepath + "\\PatentItem\\";
                if (System.IO.Directory.Exists(newpath) == false)
                {
                    System.IO.Directory.CreateDirectory(newpath);
                }
                var guid = Guid.NewGuid();
                var filename = DateTime.Now.Ticks + "." + model.UploadFile.FileName.Split('.').Last();
                var path = newpath + filename;
                model.UploadFilePath = "\\PatentItem\\" + filename;
                model.UploadFile.SaveAs(path);
            }
            model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增專利管理=" + model.Title);
                return Json(_IModelPatentManager.CreateItem(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改專利管理=" + model.ItemID + " Name=" + model.Title);
                return Json(_IModelPatentManager.UpdateItem(model, this.LanguageID, this.Account));
            }
            return Json("");
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/PatentItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/PatentItem/" + filename);
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/PatentItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/PatentItem/" + filename);
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

        #region UnitSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = mainid;
            var model = _IModelPatentManager.GetUnitModel(mainid);
            var maindata = _IModelPatentManager.Where(new ModelPatentMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
            return View(model);
        } 
        #endregion


        #region SaveUnit
        public ActionResult SaveUnit(PatentUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                model.IntroductionHtml = HttpUtility.UrlDecode(model.IntroductionHtml);
                Common.SetLogs(this.UserID, this.Account, "設定專利管理單元管理");
                return Json(_IModelPatentManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string modelid, string itemid)
        {
            var model = _IModelPatentManager.GetModelByID(modelid, itemid);
            string filepath = model.UploadFilePath;
            string oldfilename = model.UploadFileName;
            if (filepath != "")
            {
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
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