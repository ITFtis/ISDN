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
    public class MenuController : AppController
    {
        IMenuManager _IMenuManager;
        readonly SQLRepository<Img> _imgsqlrepository;
        public MenuController()
        {
            _IMenuManager = serviceinstance.MenuManager;
            _imgsqlrepository = new SQLRepository<Img>(connectionstr);
        }

        #region MainMenu
        [AuthoridUrl("Menu/MainMenu", "menutype")]
        public ActionResult MainMenu(string menutype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (menutype == "1")
            {
                ViewBag.Title = "主要選單";
                ViewBag.onlylevle1 = "N";
            }
            else if (menutype == "2")
            {
                ViewBag.Title = "上方選單";
                ViewBag.onlylevle2 = "Y";
            }
            else if (menutype == "3")
            {
                ViewBag.Title = "下方選單";
                ViewBag.onlylevle1 = "Y";
            }
            else if (menutype == "4")
            {
                ViewBag.Title = "手機版選單(主選單)";
                ViewBag.onlylevle1 = "N";
            }
            else if (menutype == "5")
            {
                ViewBag.Title = "手機版選單(上方選單)";
                ViewBag.onlylevle2 = "Y";
            }
            ViewBag.menutype = menutype;
            return View(_IMenuManager.GetMenu(this.LanguageID, menutype));
        }
        #endregion

        #region MenuEdit
        [AuthoridUrl("Menu/MainMenu", "menutype")]
        public ActionResult MenuEdit(string menuid, string menutype, string level, string parentid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (menutype == "1")
            {
                ViewBag.Title = "主要選單";
            }
            else if (menutype == "2")
            {
                ViewBag.Title = "上方選單";
            }
            else if (menutype == "3")
            {
                ViewBag.Title = "下方選單";
            }
            else if (menutype == "4")
            {
                ViewBag.Title = "手機版選單(主選單)";
            }
            else if (menutype == "5")
            {
                ViewBag.Title = "手機版選單(上方選單)";
            }
            var model = new MenuEditModel();
            if (menuid == "-1")
            {
                model.MenuType = int.Parse(menutype);
                model.MenuLevel = int.Parse(level);
                model.ParentID = int.Parse(parentid);
            }
            else
            {
                model = _IMenuManager.GetModel(menuid);
            }

            return View(model);
        }
        #endregion

        #region MenuLinkEdit
        [AuthoridUrl("Menu/MainMenu", "menutype")]
        public ActionResult MenuLinkEdit(string menuid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
           var model = _IMenuManager.GetModel(menuid);

            return View(model);
        }
        #endregion

        #region DeleteMenu
        public ActionResult DeleteMenu(string menuid)
        {
            return Json(_IMenuManager.DeleteMenu(menuid));
        }
        #endregion

        #region GetModelItem
        public ActionResult GetModelItem(string modelid)
        {
            return Json(_IMenuManager.GetModelItem(modelid,LanguageID));
        }
        #endregion

        #region SaveMenu
        public ActionResult SaveMenu(MenuEditModel model)
        {
            if (model.OpenMode == 3)
            {
                if (model.WindowWidth == null) { return Json("視窗寬度必須為整數"); }
                if (model.WindowHeight == null) { return Json("內頁風格高度必須為整數"); }
            }
            if (model.LinkMode == 2)
            {
                if (model.ModelID == 0) { return Json("請選擇程式模組"); }
                if (model.ModelItemID == 0) { return Json("請選擇程式模組項目"); }
            }
            if (model.LinkMode !=3)
            {
                model.LinkUrl = "";
            }
            if (model.ImageHeight == null) { model.ImageHeight = 219; }
            model.MenuName = HttpUtility.UrlDecode(model.MenuName);
            model.ICon = HttpUtility.UrlDecode(model.ICon);
            if (Request.IsAuthenticated)
            {
                //刪除原本檔案
                if (model.ImageFile != null)
                {
                    var fileformat = model.ImageFile.FileName.Split('.');
                    var fullfilename = model.ImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var adpath = "";
                    var checkpath = root + "\\UploadImage\\MenuImage\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.ImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\MenuImage\\" + model.ImgNameOri;
                    model.ImageFile.SaveAs(path);
                    model.ImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\MenuImage\\" + ticks + "_thumb." + fileformat.Last();
                    model.ImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "multi_lang_home" });
                    int width = 170;
                    var chartwidth = 170;
                    if (model.ImageHeight != null)
                    {
                        chartwidth = model.ImageHeight.Value;
                    }
                    if (imgdata.Count() > 0) { width = imgdata.First().width.Value; }
                    if (chartwidth > width) { chartwidth = width; };
                    var haspath = Utilities.UploadImg.uploadImgThumb(path, thumbpath, chartwidth);
                    if (haspath == "") { model.ImgNameThumb = ""; }
                    //表示有舊資料
                }

                if (model.LinkUploadFile != null)
                {
                    model.LinkUploadFileName = model.LinkUploadFile.FileName.Split('\\').Last();
                    var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                    if (uploadfilepath.IsNullorEmpty())
                    {
                        uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                    }
                    var newpath = uploadfilepath + "\\MenuFile\\";
                    if (System.IO.Directory.Exists(newpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(newpath);
                    }
                    var guid = Guid.NewGuid();
                    var filename = guid + "." + model.LinkUploadFile.FileName.Split('.').Last();
                    var path = newpath + filename;
                    model.LinkUploadFilePath = path;
                    model.LinkUploadFile.SaveAs(path);
                }
                model.LangID = int.Parse(this.LanguageID);
                if (model.ID <= 0)
                {
                    return Json(_IMenuManager.Create(model, this.Account, this.UserName));
                }
                else
                {
                    return Json(_IMenuManager.Update(model, this.Account, this.UserName));
                }
            }
            return Json("");
        }
        #endregion

        #region Menudisabled
        public ActionResult Menudisabled(string menuid)
        {
            return Json(_IMenuManager.Menudisabled(menuid));
        }
        #endregion

        #region Menueabled
        public ActionResult Menueabled(string menuid)
        {
            return Json(_IMenuManager.Menueabled(menuid));
        }
        #endregion

        #region SortNext
        public ActionResult SortNext(string menuid)
        {
            return Json(_IMenuManager.UpdateSort(int.Parse(menuid), "next", this.Account, this.UserName));
        }
        #endregion

        #region SortUp
        public ActionResult SortUp(string menuid)
        {
            return Json(_IMenuManager.UpdateSort(int.Parse(menuid), "up", this.Account, this.UserName));
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string id)
        {
            var model = _IMenuManager.GetModel(id);
            string filepath = model.LinkUploadFilePath;
            string oldfilename = model.LinkUploadFileName;
            if (filepath != "")
            {
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                TempData["model"] = model;
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        #endregion

        #region SaveMenuLinkEdit
        public ActionResult SaveMenuLinkEdit(string linkurl, string menuid)
        {

            return Json(_IMenuManager.UpdateMenuLink(linkurl, menuid, this.Account, this.UserName));
        }
        #endregion
    }
}