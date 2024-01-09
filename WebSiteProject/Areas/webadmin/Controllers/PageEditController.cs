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

    public class PageEditController : AppController
    {
        IModelPageEditManager _IPageEditManager;
        public PageEditController()
        {
            _IPageEditManager = serviceinstance.ModelPageEditManager;
        }

        #region Index
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.Title = "圖文編輯";
            Session["IsFromClick"] = "Y";
            return View();
        }
        #endregion

        #region UnitSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string modelid)
        {
            if (modelid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = modelid;
            var model = _IPageEditManager.GetUnitModel(modelid);
            return View(model);
        }
        #endregion

        #region ItemIndex
        [AuthoridUrl("Model/Index", "")]
        public ActionResult ItemIndex(string modelid)
        {
            if (modelid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = modelid;
            return View();
        } 
        #endregion

        #region PagingMain
        public ActionResult PagingMain(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            return Json(_IPageEditManager.Paging(model));
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(SearchModelBase model)
        {
            return Json(_IPageEditManager.PagingItem(model));
        }
        #endregion

        #region EditUnit
        public ActionResult EditUnit(string id,string name)
        {
            if (Request.IsAuthenticated)
            {
                var str = "";
                if (id == "-1")
                {
                    var newid = 0;
                    Common.SetLogs(this.UserID, this.Account, "新增圖文編輯管理單元名稱=" + name);
                    str = _IPageEditManager.AddUnit(name, this.LanguageID, this.Account,ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改圖文編輯單元名稱 ID=" + id + " 改為:" + name);
                    str = _IPageEditManager.UpdateUnit(name, id, this.Account);
                }
                return Json(str);
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            Common.SetLogs(this.UserID, this.Account, "刪除下列圖文編輯=" + delaccount);
            if (Request.IsAuthenticated)
            {
                return Json(_IPageEditManager.Delete(idlist, delaccount, this.LanguageID,this.Account,this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更圖文編輯單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IPageEditManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region ModelItem
        [AuthoridUrl("Model/Index", "")]
        public ActionResult ModelItem(string id, string itemid)
        {
            ViewBag.isview = Request["isview"] == null ? "" : Request["isview"];
            if (id.IsNullorEmpty()) { return RedirectToAction("Index"); }
            if (ViewBag.isview == null || ViewBag.isview == "") {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
              
            if (Session["IsFromClick"] != null) {
                ViewBag.IsFromClick = "Y";
            }
            ViewBag.ModelItemList = _IPageEditManager.GetSelectList(id);
            ViewBag.Title = "圖文編輯";
            PageEditItemModel model = null;
            if (itemid.IsNullorEmpty())
            {
                model = _IPageEditManager.GetFirstModel(id);
            }
            else
            {
                model = _IPageEditManager.GetModelByID(id, itemid.ToString());
            }
            return View(model);
        }
        #endregion

        [HttpPost]
        #region UploadImage
        public string UploadImage(HttpPostedFileBase file)
        {
            try
            {
                var fileformat = file.FileName.Split('.');
                Stream fileStream = file.InputStream;
                var mStreamer = new MemoryStream();
                mStreamer.SetLength(fileStream.Length);
                fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
                mStreamer.Seek(0, SeekOrigin.Begin);
                byte[] fileBytes = mStreamer.GetBuffer();
                Stream stream = new MemoryStream(fileBytes);
                var guid = Guid.NewGuid();
                //string result = System.Text.Encoding.UTF8.GetString(fileBytes);
                var img = System.Drawing.Bitmap.FromStream(stream);
                var root = Request.PhysicalApplicationPath;
                var filename = guid + "." + fileformat.Last();
                var checkpath = root + "\\UploadImage\\PageEdit\\";
                if (System.IO.Directory.Exists(checkpath) == false)
                {
                    System.IO.Directory.CreateDirectory(checkpath);
                }
                var path = root + "\\UploadImage\\PageEdit\\" + guid + "." + fileformat.Last();
                img.Save(path);

                //return fileBytes;
                return Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/UploadImage/PageEdit/" + filename;

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogError("UploadImageError:" + ex.Message);
                return "";
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
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/PageEdit/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/PageEdit/" + filename);
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

        #region SaveItem
        public ActionResult SaveItem(PageEditItemModel model)
        {
            if (model.UploadFile != null)
            {
                model.UploadFileName = model.UploadFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newpath = uploadfilepath + "\\PageEdit\\";
                if (System.IO.Directory.Exists(newpath) == false)
                {
                    System.IO.Directory.CreateDirectory(newpath);
                }
                var guid = Guid.NewGuid();
                var filename = DateTime.Now.Ticks + "." + model.UploadFile.FileName.Split('.').Last();
                var path = newpath + filename;
                model.UploadFilePath = "\\PageEdit\\"+ filename;
                model.UploadFile.SaveAs(path);
            }

            if (model.ImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.ImageFileOrgName = model.ImageFile.FileName.Split('\\').Last();
                var newfilename = DateTime.Now.Ticks + "_" + model.ImageFileOrgName;
                var path = root + "\\UploadImage\\PageEdit\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\PageEdit\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\PageEdit\\");
                }
                model.ImageFile.SaveAs(path);
                model.ImageFileName = newfilename;
            }
            model.HtmlContent= HttpUtility.UrlDecode(model.HtmlContent);
            model.LangID = LanguageID;
            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增圖文編輯=" + model.ItemID);
                return Json(_IPageEditManager.CreatePageEdit(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改圖文編輯ID=" + model.ItemID );
                return Json(_IPageEditManager.EditPageItem(model,  this.Account));
            }
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string modelid,string itemid)
        {
            var model = _IPageEditManager.GetModelByID(modelid, itemid);
            string filepath = model.UploadFilePath;
            string oldfilename = model.UploadFileName;
            var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            if (uploadfilepath.IsNullorEmpty())
            {
                uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
            }
            if (filepath != "")
            {
                //取得檔案名稱
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                //讀成串流
                Stream iStream = new FileStream(uploadfilepath+filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //回傳出檔案
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        #endregion

        #region SetStatus
        public ActionResult SetStatus(string id, bool status)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IPageEditManager.UpdateStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region EditItemUnit
        public ActionResult EditItemUnit(string modelid, string id, string name)
        {
            if (Request.IsAuthenticated)
            {
                var str = "";
                if (id == "-1")
                {
                    str = _IPageEditManager.AddItemUnit(modelid,name, this.Account);
                }
                else
                {
                    str = _IPageEditManager.UpdateItemUnit(name, id, this.Account);
                }
                return Json(str);
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid,int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改圖文編輯排序ID=" + id + " sequence=" + seq);
                return Json(_IPageEditManager.UpdateItemSeq(modelid,id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemDelete
        public ActionResult SetItemDelete(string modelid,string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除圖文編輯=" + delaccount);
                return Json(_IPageEditManager.DeleteItem(idlist, delaccount, modelid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(PageUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定圖文編輯單元管理");
                return Json(_IPageEditManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/PageEdit/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/PageEdit/" + filename);
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