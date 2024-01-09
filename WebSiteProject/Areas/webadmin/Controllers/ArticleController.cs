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

    public class ArticleController : AppController
    {
        IModelArticleManager _IModelArticleManager;
        public ArticleController()
        {
            _IModelArticleManager = serviceinstance.ModelArticleManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            Session["IsFromClick"] = "Y";
            ViewBag.Title = "文章管理";
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = mainid;
            var model = _IModelArticleManager.GetUnitModel(mainid);
            var maindata = _IModelArticleManager.Where(new ModelArticleMain()
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
            ViewBag.baseurl = VirtualPathUtility.ToAbsolute("~/UploadImage/Article/");
            ViewBag.GroupListUrl = "Article/GroupList?itemid=" + mainid;
            return View();
        }

        [AuthoridUrl("Model/Index", "")]
        public ActionResult GroupSubEdit(string mainid,string groupid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = mainid;
            ViewBag.groupid = groupid;
            return View();
        }

        [AuthoridUrl("Model/Index", "")]
        public ActionResult ArticleEdit(string mainid, string itemid="-1")
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            ViewBag.isview = Request["isview"] == null ? "" : Request["isview"];
            if (ViewBag.isview == null || ViewBag.isview == "")
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
            ViewBag.grouplist = _IModelArticleManager.GetAllGroupSelectList(mainid);
            ArticleEditModel model = null;
            model = _IModelArticleManager.GetModelByID(mainid, itemid);
            ViewBag.GroupList = _IModelArticleManager.GetAllGroupList(mainid).ToArray();
            ViewBag.GroupSubList = _IModelArticleManager.GetAllGroupSubList(mainid).ToArray();
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
                    Common.SetLogs(this.UserID, this.Account, "新增文章管理單元名稱=" + name);
               
                    str = _IModelArticleManager.AddUnit(name, this.LanguageID, this.Account,ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改文章管理單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IModelArticleManager.UpdateUnit(name, mainid, this.Account);
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
            return Json(_IModelArticleManager.Paging(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更文章管理單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IModelArticleManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列文章管理=" + delaccount);
                return Json(_IModelArticleManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
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
            var grouplist = _IModelArticleManager.GetAllGroupSelectList(mainid);
            grouplist.Insert(0, new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            ViewBag.grouplist = grouplist;
            ViewBag.mainid = mainid;
           var maindata = _IModelArticleManager.Where(new ModelArticleMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
          
            return View();
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(ArticleSearchModel model)
        {
            return Json(_IModelArticleManager.PagingItem(model.ModelID.ToString(),model));
        }
        #endregion

        //group
        #region PagingGroup
        public ActionResult PagingGroup(SearchModelBase model)
        {
            return Json(_IModelArticleManager.PagingGroup(model));
        }
        #endregion

        //group
        #region PagingGroup
        public ActionResult PagingSubGroup(SearchModelBase model)
        {
            return Json(_IModelArticleManager.PagingSubGroup(model));
        }
        #endregion


        #region EditGroup
        public ActionResult EditGroup(string name, string id,string mainid,bool hassubgroup, HttpPostedFileBase ImageFile)
        {
            var ImageFileName = "";
            var ImageFileOrgName = "";
            if (ImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                ImageFileOrgName = ImageFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + ImageFileOrgName;
                var path = root + "\\UploadImage\\Article\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\Article\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\Article\\");
                }
                ImageFile.SaveAs(path);
                ImageFileName = newfilename;
            }


            if (id == "-1" || id.IsNullorEmpty())
            {
                Common.SetLogs(this.UserID, this.Account, "新增文章管理群組名稱=" + name + " mainid=" + mainid);
                return Json(_IModelArticleManager.EditGroup(name, id, mainid,this.Account, ImageFileName, ImageFileOrgName, hassubgroup));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改文章管理群組名稱=" + name + " id=" + id);
                return Json(_IModelArticleManager.EditGroup(name, id, mainid,this.Account, ImageFileName, ImageFileOrgName, hassubgroup));
            }

        }
        #endregion

        #region EditSubGroup
        public ActionResult EditSubGroup(string name, string id, string mainid, HttpPostedFileBase ImageFile,string groupid)
        {
            var ImageFileName = "";
            var ImageFileOrgName = "";
            if (ImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                ImageFileOrgName = ImageFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + ImageFileOrgName;
                var path = root + "\\UploadImage\\Article\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\Article\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\Article\\");
                }
                ImageFile.SaveAs(path);
                ImageFileName = newfilename;
            }


            if (id == "-1" || id.IsNullorEmpty())
            {
                Common.SetLogs(this.UserID, this.Account, "新增文章管理群組名稱=" + name + " mainid=" + mainid);
                return Json(_IModelArticleManager.EditSubGroup(name, id, mainid, this.Account, ImageFileName, ImageFileOrgName, groupid));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改文章管理群組名稱=" + name + " id=" + id);
                return Json(_IModelArticleManager.EditSubGroup(name, id, mainid, this.Account, ImageFileName, ImageFileOrgName, groupid));
            }

        }
        #endregion

        #region EditGroupSeq
        public ActionResult EditGroupSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更文章管理群組排序MainID=" + mainid + " ID =" + id + "排序=" + seq);
                return Json(_IModelArticleManager.UpdateGroupSeq(id.Value, seq, mainid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region EditGroupSubSeq
        public ActionResult EditGroupSubSeq(int? id, int seq, string groupid)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelArticleManager.UpdateGroupSubSeq(id.Value, seq, groupid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetGroupDelete
        public ActionResult SetGroupDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除文章管理群組=" + delaccount);
                return Json(_IModelArticleManager.DeleteGroup(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupSubDelete
        public ActionResult SetGroupSubDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除文章管理群組=" + delaccount);
                return Json(_IModelArticleManager.SetGroupSubDelete(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupStatus
        public ActionResult SetGroupStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定文章管理群組id=" + id + "為" + status);
                return Json(_IModelArticleManager.UpdateGroupStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupSubStatus
        public ActionResult SetGroupSubStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定文章管理群組id=" + id + "為" + status);
                return Json(_IModelArticleManager.UpdateGroupSubStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion



        //==column
        #region PagingColumn
        public ActionResult PagingColumn(SearchModelBase model)
        {
            return Json(_IModelArticleManager.ColumnPaging(model));
        }
        #endregion

        #region SetColumnStatus
        public ActionResult SetColumnStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelArticleManager.UpdateColumnStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditColumnSeq
        public ActionResult EditColumnSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelArticleManager.UpdateColumnSeq(id.Value, seq,  this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(ArticleUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定文章管理單元管理");
                model.Summary = HttpUtility.UrlDecode(model.Summary);
                model.SummaryIn = HttpUtility.UrlDecode(model.SummaryIn);
                return Json(_IModelArticleManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        //===
        #region SEOSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult SEOSetting(string modelid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = modelid;
            return View(_IModelArticleManager.GetSEO(modelid));
        }
        #endregion

        #region SaveSEO
        public ActionResult SaveSEO(SEOViewModel model)
        {
            Common.SetLogs(this.UserID, this.Account, "修改文章管理SEO");
            return Json(_IModelArticleManager.SaveSEO(model, this.LanguageID));
        } 
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string modelid, string itemid)
        {
            var model = _IModelArticleManager.GetModelByID(modelid, itemid);
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

        #region SaveItem
        public ActionResult SaveItem(ArticleEditModel model)
        {
            if (model.UploadFile != null)
            {
                model.UploadFileName = model.UploadFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newpath = uploadfilepath + "\\ArticleItem\\";
                if (System.IO.Directory.Exists(newpath) == false)
                {
                    System.IO.Directory.CreateDirectory(newpath);
                }
                var guid = Guid.NewGuid();
                var filename = DateTime.Now.Ticks + "." + model.UploadFile.FileName.Split('.').Last();
                var path = newpath + filename;
                model.UploadFilePath = "\\ArticleItem\\" + filename;
                model.UploadFile.SaveAs(path);
            }

            if (model.ImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.ImageFileOrgName = model.ImageFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.ImageFileOrgName;
                var path = root + "\\UploadImage\\ArticleItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\ArticleItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\ArticleItem\\");
                }
                model.ImageFile.SaveAs(path);
                model.ImageFileName = newfilename;
            }

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
                var path = root + "\\UploadImage\\ArticleItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\ArticleItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\ArticleItem\\");
                }
                model.RelateImageFile.SaveAs(path);
                model.RelateImageName = newfilename;
            }

            model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
            model.Title = HttpUtility.UrlDecode(model.Title);
            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增訊息管理=" + model.Title);
                return Json(_IModelArticleManager.CreateItem(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改文章管理ID=" + model.ItemID + " Name=" + model.Title);
                return Json(_IModelArticleManager.UpdateItem(model, this.LanguageID, this.Account));
            }
        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid,int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改文章管理排序ID=" + id + " sequence=" + seq);
                return Json(_IModelArticleManager.UpdateItemSeq(modelid,id, seq,this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemStatus
        public ActionResult SetItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改文章管理狀態ID=" + id + " status=" + status);
                return Json(_IModelArticleManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemDelete
        public ActionResult SetItemDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除文章管理=" + delaccount);
                return Json(_IModelArticleManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/ArticleItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/ArticleItem/" + filename);
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/ArticleItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/ArticleItem/" + filename);
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

        #region GetSubGroup
        public ActionResult GetSubGroup(string groupid,string mainid)
        {
            var allsubgroup = _IModelArticleManager.GetAllGroupSubList(mainid);
            allsubgroup = allsubgroup.Where(v => v.Group_ID.ToString() == groupid).ToList();
            if (allsubgroup.Count() > 0)
            {
                var sb = new System.Text.StringBuilder();
                sb.Append("<option value=''>全部</option>");
                foreach (var v in allsubgroup) {
                    sb.Append("<option value='" + v.ID+"'>"+ v .Group_Name+"</option>");
                }
                return Json(sb.ToString());
            }
            else {
                return Json("");
            }
           
        }
        #endregion
        
    }
}