using Services.Interface;
using Services.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;
namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class ModelController : AppController
    {
        IModelManager _IModelManager;
        public ModelController()
        {
            _IModelManager = new ModelManager(connectionstr);
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View();
        }
        #region IndexSetting
        [AuthoridUrl("Model/IndexSetting", "")]
        public ActionResult IndexSetting()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _IModelManager.GetPageIndexSettingModel(this.LanguageID);
            return View(model);
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveIndexSetting(PageIndexSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                model.HtmlContent= HttpUtility.UrlDecode(model.HtmlContentCode);
                return Json(_IModelManager.SetPageIndexSettingModel(model,this.LanguageID, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region UploadImage
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
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