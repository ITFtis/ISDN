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
    [Authorize]
    public class ActiveController : AppController
    {
        IModelActiveManager _IModelActiveManager;
        
        public ActiveController()
        {
            _IModelActiveManager = serviceinstance.ModelActiveManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            Session["IsFromClick"] = "Y";
            ViewBag.Title = "活動管理";
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = mainid;
            var model = _IModelActiveManager.GetUnitModel(mainid);
            var maindata = _IModelActiveManager.Where(new  ModelActiveEditMain  ()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
            return View(model);
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult GroupEdit(string mainid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = mainid;
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult ActiveEdit(string mainid, string itemid="-1")
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            ViewBag.isview = Request["isview"] == null ? "" : Request["isview"];
            if (ViewBag.isview == null || ViewBag.isview == "")
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
            ViewBag.grouplist = _IModelActiveManager.GetAllGroupSelectList(mainid);
            ActiveEditModel model = null;
            model = _IModelActiveManager.GetModelByID(mainid, itemid);
            return View(model);
        }

        //====
        #region EditUnit
        public ActionResult EditUnit(string mainid, string name)
        {
            if (Request.IsAuthenticated)
            {
                var str = "";
                int newid = 0;
                if (mainid == "-1" ||string.IsNullOrEmpty(mainid))
                {
                    Common.SetLogs(this.UserID, this.Account, "新增活動管理單元名稱=" + name);
               
                    str = _IModelActiveManager.AddUnit(name, this.LanguageID, this.Account,ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改活動管理單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IModelActiveManager.UpdateUnit(name, mainid, this.Account);
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
            return Json(_IModelActiveManager.Paging(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更活動管理單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IModelActiveManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列訊息管理=" + delaccount);
                return Json(_IModelActiveManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion
        //==
        #region ModelItem
        [AuthoridUrl("Model/Index", "")]
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ModelItem(string mainid,string menuindex)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (Session["IsFromClick"] != null)
            {
                ViewBag.IsFromClick = "Y";
            }
            var grouplist = _IModelActiveManager.GetAllGroupSelectList(mainid);
            grouplist.Insert(0, new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            ViewBag.grouplist = grouplist;
            ViewBag.mainid = mainid;
           var maindata = _IModelActiveManager.Where(new ModelActiveEditMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
          
            return View();
        }
        #endregion

        #region PagingItem

        public ActionResult PagingItem(ActiveSearchModel model)
        {
            return Json(_IModelActiveManager.PagingItem(model.ModelID.ToString(),model));
        }
        #endregion

        //group
        #region PagingGroup
        public ActionResult PagingGroup(SearchModelBase model)
        {
            return Json(_IModelActiveManager.PagingGroup(model));
        }
        #endregion

        #region EditGrou
        public ActionResult EditGroup(string name, string id,string mainid)
        {
            if (id == "-1" || id.IsNullorEmpty())
            {
                Common.SetLogs(this.UserID, this.Account, "新增活動管理群組名稱=" + name + " mainid=" + mainid);
                return Json(_IModelActiveManager.EditGroup(name, id, mainid,this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改活動管理群組名稱=" + name + " id=" + id);
                return Json(_IModelActiveManager.EditGroup(name, id, mainid,this.Account));
            }

        }
        #endregion

        #region EditGroupSeq
        public ActionResult EditGroupSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更活動管理群組排序MainID=" + mainid + " ID =" + id + "排序=" + seq);
                return Json(_IModelActiveManager.UpdateGroupSeq(id.Value, seq, mainid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetGroupDelete
        public ActionResult SetGroupDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除活動管理群組=" + delaccount);
                return Json(_IModelActiveManager.DeleteGroup(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupStatus
        public ActionResult SetGroupStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定活動管理群組id=" + id + "為" + status);
                return Json(_IModelActiveManager.UpdateGroupStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        //==column
        #region PagingColumn
        public ActionResult PagingColumn(SearchModelBase model)
        {
            return Json(_IModelActiveManager.ColumnPaging(model));
        }
        #endregion

        #region SetColumnStatus
        public ActionResult SetColumnStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelActiveManager.UpdateColumnStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditColumnSeq
        public ActionResult EditColumnSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelActiveManager.UpdateColumnSeq(id.Value, seq,  this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(ActiveUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定活動管理單元管理");
                return Json(_IModelActiveManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        //===
        #region SEOSetting
        public ActionResult SEOSetting(string modelid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = modelid;
            return View(_IModelActiveManager.GetSEO(modelid));
        }
        #endregion

        #region SaveSEO
        public ActionResult SaveSEO(SEOViewModel model)
        {
            Common.SetLogs(this.UserID, this.Account, "修改活動管理SEO");
            return Json(_IModelActiveManager.SaveSEO(model, this.LanguageID));
        } 
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string fileid)
        {
            var model = _IModelActiveManager.GetActiveFiles(fileid);
            string filepath = "";
            string oldfilename = "";
            if (model.FilePath.IsNullorEmpty() == false)
            {
                //取得檔案名稱
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadFile";
                }
                filepath = uploadfilepath + model.FilePath;
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

        #region SaveItem
        public ActionResult SaveItem(ActiveEditModel model)
        {
            var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            if (uploadfilepath.IsNullorEmpty())
            {
                uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
            }

            if (model.ImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.ImageFileOrgName = model.ImageFile.FileName.Split('\\').Last();
                var newfilename = DateTime.Now.Ticks + "_" + model.ImageFileOrgName;
                var path = root + "\\UploadImage\\ActiveItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\ActiveItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\ActiveItem\\");
                }
                model.ImageFile.SaveAs(path);
                model.ImageFileName = newfilename;
            }

            if (model.RelateImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.RelateImageFileOrgName = model.RelateImageFile.FileName.Split('\\').Last();
                var newfilename = DateTime.Now.Ticks + "_" + model.RelateImageFileOrgName;
                var path = root + "\\UploadImage\\ActiveItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\ActiveItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\ActiveItem\\");
                }
                model.RelateImageFile.SaveAs(path);
                model.RelateImageName = newfilename;
            }
            model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
            model.Title = HttpUtility.UrlDecode(model.Title);
            model.Introduction = HttpUtility.UrlDecode(model.Introduction);
            
            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增活動管理=" + model.Title);
                return Json(_IModelActiveManager.CreateItem(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改活動管理ID=" + model.ItemID + " Name=" + model.Title);
                return Json(_IModelActiveManager.UpdateItem(model, this.LanguageID, this.Account));
            }

        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid,int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改活動管理排序ID=" + id + " sequence=" + seq);
                return Json(_IModelActiveManager.UpdateItemSeq(modelid,id, seq,this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemStatus
        public ActionResult SetItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改活動管理狀態ID=" + id + " status=" + status);
                return Json(_IModelActiveManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemDelete
        public ActionResult SetItemDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除活動管理=" + delaccount);
                return Json(_IModelActiveManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

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
                var root = Request.PhysicalApplicationPath + "/UploadImage/ActiveItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/ActiveItem/" + filename);
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/ActiveItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/ActiveItem/" + filename);
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
    }
}