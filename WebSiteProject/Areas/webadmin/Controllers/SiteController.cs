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
    public class SiteController : AppController
    {
        ISiteLayoutManager _ISiteLayoutManager;
        IMenuManager _IMenuManager;
        IModelMessageManager _IMessageManager;
        IModelActiveManager _IModelActiveManager;
        IModelFileDownloadManager _IModelFileDownloadManager;
        IModelLinkManager _IModelLinkManager;
        ISiteConfigManager _ISiteConfigManager;
        readonly SQLRepository<Img> _imgsqlrepository;
        public SiteController()
        {
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IMessageManager = serviceinstance.MessageManager;
            _IModelActiveManager = serviceinstance.ModelActiveManager;
            _IModelLinkManager = serviceinstance.ModelLinkManager;
            _imgsqlrepository = new SQLRepository<Img>(connectionstr);
            _ISiteConfigManager = serviceinstance.SiteConfigManager;
            _IModelFileDownloadManager = serviceinstance.ModelFileDownloadManager;
        }

        #region SiteLayout
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult SiteLayout(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (string.IsNullOrEmpty(stype)) { stype = "P"; ; }
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            if (stype == "M")
            {
                ViewBag.Title = "版面管理(手機板)";
            }
            else
            {
                ViewBag.Title = "網站版面資訊設定";
            }

            return View(model);
        }
        #endregion

        #region FowardSetting
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult FowardSetting(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            return View(model);
        }
        #endregion

        #region PrintSetting
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult PrintSetting(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            return View(model);
        }
        #endregion

        #region Page404Setting
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult Page404Setting(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            return View(model);
        }
        #endregion

        #region SaveSiteLayout
        public ActionResult SaveSiteLayout(SiteLayoutModel model)
        {
            if (Request.IsAuthenticated)
            {
                if (model.LogoImageFile != null)
                {
                    var fileformat = model.LogoImageFile.FileName.Split('.');
                    var fullfilename = model.LogoImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.LogoImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.LogoImgNameOri;
                    model.LogoImageFile.SaveAs(path);
                    model.LogoImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.LogoImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "logo" });
                    int height = 80;
                    var chartheight = 80;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };
                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.LogoImgNameThumb = ""; }
                }
                else {
                    model.LogoImgShowName = "";
                    model.LogoImgNameOri = "";
                    model.LogoImgNameThumb = "";
                }

                if (model.InnerLogoImageFile != null)
                {
                    var fileformat = model.InnerLogoImageFile.FileName.Split('.');
                    var fullfilename = model.InnerLogoImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.InnerLogoImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.InnerLogoImgNameOri;
                    model.InnerLogoImageFile.SaveAs(path);
                    model.InnerLogoImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.InnerLogoImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "logo" });
                    int height = 80;
                    var chartheight = 80;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };
                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.InnerLogoImgNameThumb = ""; }
                }
                model.PublishContent = HttpUtility.UrlDecode(model.PublishContent);
                model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
                model.LangID = LanguageID;
                return Json(_ISiteLayoutManager.EditSiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion
 
        #region SaveFowardSiteLayout
        public ActionResult SaveFowardSiteLayout(SiteLayoutModel model)
        {

            if (Request.IsAuthenticated)
            {
                if (model.FowardImageFile != null)
                {
                    var fileformat = model.FowardImageFile.FileName.Split('.');
                    var fullfilename = model.FowardImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.FowardImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.FowardImgNameOri;
                    model.FowardImageFile.SaveAs(path);
                    model.FowardImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.FowardImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "site_forward" });
                    int height = 150;
                    var chartheight = 150;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };

                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.FowardImgNameThumb = ""; }
                }

                model.FowardHtmlContent = HttpUtility.UrlDecode(model.FowardHtmlContent);
                return Json(_ISiteLayoutManager.EditFowardSiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion

        #region SavePrintSiteLayout
        public ActionResult SavePrintSiteLayout(SiteLayoutModel model)
        {

            if (Request.IsAuthenticated)
            {
                if (model.PrintImageFile != null)
                {
                    var fileformat = model.PrintImageFile.FileName.Split('.');
                    var fullfilename = model.PrintImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.PrintImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.PrintImgNameOri;
                    model.PrintImageFile.SaveAs(path);
                    model.PrintImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.PrintImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "site_forward" });
                    int height = 150;
                    var chartheight = 150;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };

                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.PrintImgNameThumb = ""; }
                }

                model.PrintHtmlContent = HttpUtility.UrlDecode(model.PrintHtmlContent);
                return Json(_ISiteLayoutManager.EditPrintSiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion

        #region Save404SiteLayout
        public ActionResult Save404SiteLayout(SiteLayoutModel model)
        {

            if (Request.IsAuthenticated)
            {

                model.Page404HtmlContent = HttpUtility.UrlDecode(model.Page404HtmlContent);
                return Json(_ISiteLayoutManager.EditPage404SiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion

        #region SiteLangText
        [AuthoridUrl("Site/SiteLangText", "")]
        public ActionResult SiteLangText()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLangText(LanguageID);
            return View(model);
        }
        #endregion

        #region SaveSiteLangText
        public ActionResult SaveSiteLangText(SiteLangTextModel model)
        {
            var str = _ISiteLayoutManager.SaveSiteLangText(model, LanguageID);
            Common.SetAllLangKey();
            return Json(str);
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/SiteLayout/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/SiteLayout/" + filename);
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/SiteLayout/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/SiteLayout/" + filename);
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

        //說明會

        #region PageSeminar
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult PageSeminar(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (string.IsNullOrEmpty(stype)) { stype = "P"; ; }
            PageSeminarModel model = _ISiteLayoutManager.GetPageSeminarModel();
            return View(model);
        }
        #endregion

        #region SavePageSeminar
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult SavePageSeminar(PageSeminarModel model)
        {
            model.Title = HttpUtility.UrlDecode(model.Title);
            model.Introduction = HttpUtility.UrlDecode(model.Introduction);
            if (model.UploadFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.StyleUploadFileNameOrg = model.UploadFile.FileName.Split('\\').Last();
                var newfilename = DateTime.Now.Ticks + "_" + model.StyleUploadFileNameOrg;
                var path = root + "\\UploadImage\\PageLayout\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\PageLayout\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\PageLayout\\");
                }
                model.StyleUploadFilePath = path;
                model.UploadFile.SaveAs(path);
                model.StyleUploadFileName = newfilename;
            }
            else {
                model.StyleUploadFileName = "";
            }
            string result = _ISiteLayoutManager.SavePageSeminar(model);
            return Json(result);
        }
        #endregion

        #region PageNewsEdit
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult PageNewsEdit(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetPageNewEdit();
            return View(model);
        }
        #endregion

        #region GetModelItem
        public ActionResult GetModelItem(string modelid)
        {
            return Json(_IMenuManager.GetModelItem(modelid, LanguageID,false));
        }
        #endregion

        #region GetModelItemList
        public ActionResult GetModelItemList(SearchModelBase model)
        {
            if (model.Key == "2") {
                return Json(_IMessageManager.PagingItem(model.ModelID.ToString(), new MessageSearchModel() {
                   Limit=model.Limit,
                    Sort=model.Sort,
                      NowPage=model.NowPage,
                       Offset=model.Offset,
                        Order=model.Order
                }));
            }
            else if (model.Key == "3")
            {
                return Json(_IModelActiveManager.PagingItem(model.ModelID.ToString(), new  ActiveSearchModel()
                {
                    Limit = model.Limit,
                    Sort = model.Sort,
                    NowPage = model.NowPage,
                    Offset = model.Offset,
                    Order = model.Order
                }));
            }
            else if (model.Key == "4")
            {
                return Json(_IModelFileDownloadManager.PagingItem(model.ModelID.ToString(), new  FileDownloadSearchModel()
                {
                    Limit = model.Limit,
                    Sort = model.Sort,
                    NowPage = model.NowPage,
                    Offset = model.Offset,
                    Order = model.Order
                }));
            }

            return Json("");
        }
        #endregion

        #region SavePageNewEdit
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult SavePageNewEdit(PageNewsEditModel model)
        {
            if (Request.IsAuthenticated)
            {
                string result = _ISiteLayoutManager.SavePageNewEdit(model);
                return Json(result);
            }
            else {
                return Json("請重新登入後再操作");
            }
          
        }
        #endregion

        #region PageActiveEdit
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult PageActiveEdit(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            //ViewBag.moreinfo = _ISiteLayoutManager.GetActiveMoreInfo();
            var model = _ISiteLayoutManager.GetPageActiveEdit();
            return View(model);
        }
        #endregion

        #region SavePageActiveEdit
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult SavePageActiveEdit(PageActiveEditModel model)
        {
            if (Request.IsAuthenticated)
            {
                string result = _ISiteLayoutManager.SavePageActiveEdit(model);
                return Json(result);
            }
            else
            {
                return Json("請重新登入後再操作");
            }

        }
        #endregion

        #region PageJournalEdit
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult PageJournalEdit(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetPageJournalEdit();
            return View(model);
        }
        #endregion

        #region SavePageJournalEdit
        [AuthoridUrl("Site/PageLayout", "stype")]
        public ActionResult SavePageJournalEdit(PageJournalEditModel model)
        {
            if (Request.IsAuthenticated)
            {
                string result = _ISiteLayoutManager.SavePageJournalEdit(model);
                return Json(result);
            }
            else
            {
                return Json("請重新登入後再操作");
            }

        }
        #endregion

        #region PagingActiveItem
        public ActionResult PagingActiveItem(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            model.Key = "Active";
            return Json(_IModelLinkManager.PagingItem(model.ModelID.ToString(), model));
        }
        #endregion

        #region SaveActiveMoreInfo
        public ActionResult SaveActiveMoreInfo(string url)
        {
            _ISiteLayoutManager.SaveActiveMoreInfo(url);
            return Json("");
        }
        #endregion

        #region SaveNewsMoreInfo
        public ActionResult SaveNewsMoreInfo(string url)
        {
            _ISiteLayoutManager.SaveNewsMoreInfo(url);
            return Json("");
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "stype")]
        #region LinkEdit
        public ActionResult LinkEdit(string itemid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            LinkEditModel model = null;
            if (itemid.IsNullorEmpty())
            {
                model = new LinkEditModel();
            }
            else
            {
                model = _IModelLinkManager.GetModelByID("", itemid);
            }
            return View(model);
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "stype")]

        #region LinkNewEdit
        public ActionResult LinkNewEdit(string itemid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            LinkEditModel model = null;
            if (itemid.IsNullorEmpty())
            {
                model = new LinkEditModel();
            }
            else
            {
                model = _IModelLinkManager.GetModelByID("", itemid);
            }
            return View(model);
        }
        #endregion

        #region SaveItem
        public ActionResult SaveItem(LinkEditModel model)
        {
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
                var path = root + "\\UploadImage\\LinkItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\LinkItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\LinkItem\\");
                }
                model.ImageFile.SaveAs(path);
                model.ImageFileName = newfilename;
            }

            model.Title = HttpUtility.UrlDecode(model.Title);
            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增網站連結=" + model.Title);
                return Json(_IModelLinkManager.CreateItem(model, this.LanguageID, this.Account));
                return Json(_IModelLinkManager.CreateItem(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "新增網站連結=" + model.ItemID + " Name=" + model.Title);
                return Json(_IModelLinkManager.UpdateItem(model, this.LanguageID, this.Account));
            }
        }
        #endregion

        #region UpdateLinkItemSeq
        public ActionResult UpdateLinkItemSeq(int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改網站連結排序ID=" + id + " sequence=" + seq);
                return Json(_IModelLinkManager.UpdateItemSeq(int.Parse(this.LanguageID), id, seq, this.Account, this.UserName, type));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetLinkItemStatus
        public ActionResult SetLinkItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改網站連結狀態ID=" + id + " status=" + status);
                return Json(_IModelLinkManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemLinkDelete
        public ActionResult SetItemLinkDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除網站連結=" + delaccount);
                return Json(_IModelLinkManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region PagingNewsItem
        public ActionResult PagingNewsItem(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            model.Key = "News";
            return Json(_IModelLinkManager.PagingItem(model.ModelID.ToString(), model));
        }
        #endregion
    }
}