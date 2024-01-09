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

namespace WebSiteProject.Areas.webadmin.Controllers
{
    [Authorize]
    public class ADController : AppController
    {
        IADManager _IADManager;
        readonly SQLRepository<Img> _imgsqlrepository;
        public ADController()
        {
            _imgsqlrepository = new SQLRepository<Img>(connectionstr);
        }

        #region ADEdit
        [AuthoridUrl("AD/Index", "type,stype")]
        public ActionResult ADEdit(string type,string id,string stype)
        {
            if (string.IsNullOrEmpty(stype)) { stype = "P"; }
            if (type != null) { Session["ADType"] = type; }
            else
            {
                if (Session["ADType"] != null) { type = Session["ADType"].ToString(); }
            }
            if (type == null) { return RedirectToAction("Index", "Home"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = new ADEditModel();
          
            var imageadpath = "";
            var pathstr = "";
            var prestr = stype == "M" ? "手機板-" : "";
            if (type == "right")
            {
                _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));       
                ViewBag.Title = "輪播廣告管理("+ prestr+"快速選單)";
                imageadpath = "ad_right";
                pathstr = "ADRight";
            }
            else if (type == "rightdown")
            {
                _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                ViewBag.Title = "輪播廣告管理("+ prestr+"書籍專區)";
                imageadpath = "ad_rightdown";
                pathstr = "ADRightDown";
            }
            else if (type == "main")
            {
                _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
                ViewBag.Title = "輪播廣告管理("+ prestr+"主廣告)";
                imageadpath = "ad_main";
                pathstr = "ADMain";
            }
            else if (type == "down")
            {
                _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                ViewBag.Title = "輪播廣告管理("+ prestr+"友會連結)";
                imageadpath = "ad_down";
                pathstr = "ADDown";
            }
            else if (type == "center")
            {
                _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                ViewBag.Title = "輪播廣告管理("+ prestr+"橫幅廣告)";
                imageadpath = "ad_center";
                pathstr = "ADCenter";
            }
            else if (type == "mobile")
            {
                _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                ViewBag.Title = "輪播廣告管理(手機版)";
                imageadpath = "ad_mobile";
                pathstr = "ADMobile";
            }
            else if (type == "mobileblock")
            {
                _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                ViewBag.Title = "輪播廣告管理(手機版-各區塊)";
                imageadpath = "ad_mobileblock";
                pathstr = "ADMobileBlock";
            }
            if (id.IsNullorEmpty()) {
                var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { imageadpath });
                if (imgdata.Count() > 0) {
                    model.AD_Height = imgdata.First().height;
                    model.AD_Width = imgdata.First().width;
                }
                return View(model);
            }
            model = _IADManager.GetModel(id);
            model.ImageUrl = Url.Content("~/UploadImage/"+ pathstr +"/" + model.Img_Name_Thumb);
            model.Type = type;
            model.SType = stype;
            return View(model);
        }
        #endregion


        #region Index
        [AuthoridUrl("AD/Index", "type,stype")]
        public ActionResult Index(string type,string stype)
        {
            if (string.IsNullOrEmpty(stype)) { stype = "P"; }
            if (type != null){Session["ADType"] = type; }else {
                if (Session["ADType"] != null) { type = Session["ADType"].ToString(); }
            }
            if (type == null) { return RedirectToAction("Index", "Home"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.Type = type;
            ViewBag.stype = stype;
            if (type == "right")
            {
                _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
                ViewBag.Title = "輪播廣告管理(快速選單)";
                if (stype == "M") { ViewBag.Title = "手機板輪播廣告管理(手機版-快速選單)"; }
            }
            else if (type == "rightdown")
            {
                _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                ViewBag.Title = "輪播廣告管理(書籍專區)";
                if (stype == "M") { ViewBag.Title = "輪播廣告管理(手機版-書籍專區)"; }
            }
            else if (type == "main")
            {
                _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));  
                ViewBag.Title = "輪播廣告管理(主廣告)";
                if (stype == "M") { ViewBag.Title = "輪播廣告管理(手機版-主廣告)"; }
            }
            else if (type == "down")
            {
                _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                ViewBag.Title = "輪播廣告管理(友會連結)";
                if (stype == "M") { ViewBag.Title = "輪播廣告管理(手機版-友會連結)"; }
            }
            else if (type == "center")
            {
                _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                ViewBag.Title = "輪播廣告管理(橫幅廣告)";
                if (stype == "M") { ViewBag.Title = "輪播廣告管理(手機版-橫幅廣告)"; }
            }
            else if (type == "mobile")
            {
                _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                ViewBag.Title = "輪播廣告管理(手機版)";
            }
            else if (type == "mobileblock")
            {
                _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                ViewBag.Title = "輪播廣告管理(手機版-各區塊)";
            }
            var adset = _IADManager.GetADSet(this.LanguageID, type, stype);
            ViewBag.MaxNum = adset.Max_Num;
            return View();
        } 
        #endregion

        #region Paging
        public ActionResult Paging(ADSearchModel searchModel)
        {
            searchModel.Lang_ID = this.LanguageID;
            if (searchModel.ADType == "right")
            {
                _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
            }
            else if (searchModel.ADType == "rightdown")
            {
                _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
            }
            else if (searchModel.ADType == "main")
            {
                _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
            }
            else if (searchModel.ADType == "down")
            {
                _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
            }
            else if (searchModel.ADType == "center")
            {
                _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
            }
            else if (searchModel.ADType == "mobile")
            {
                _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
            }
            else if (searchModel.ADType == "mobileblock")
            {
                _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
            }
            return Json(_IADManager.Paging(searchModel));
        }
        #endregion

        #region SetMaxValue
        public ActionResult SetMaxValue(string maxnum,string type,string stype)
        {
            if (string.IsNullOrEmpty(stype)) { stype = "P"; }
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                if (type == "right")
                {
                    _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
                }
                else if (type == "rightdown")
                {
                    _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                }
                else if (type == "main")
                {
                    _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
                }
                else if (type == "down")
                {
                    _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                }
                else if (type == "center")
                {
                    _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                }
                else if (type == "mobile")
                {
                    _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                }
                else if (type == "mobileblock")
                {
                    _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                }
                return Json(_IADManager.SetMaxADCount(this.LanguageID, type, maxnum, stype));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region EditADSeq
        public ActionResult EditADSeq(int? id, int seq,string type)
        {
            if (Request.IsAuthenticated)
            {
                if (type == "right")
                {
                    _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
                }
                else if (type == "rightdown")
                {
                    _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                }
                else if (type == "main")
                {
                    _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
                }
                else if (type == "down")
                {
                    _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                }
                else if (type == "center")
                {
                    _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                }
                else if (type == "mobile")
                {
                    _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                }
                else if (type == "mobileblock")
                {
                    _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                }
                return Json(_IADManager.UpdateSeq(id.Value,seq, type,this.Account,this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetADDelete
        public ActionResult SetADDelete(string[] idlist, string delaccount,string type)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                if (type == "right")
                {
                    _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
                }
                else if (type == "rightdown")
                {
                    _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                }
                else if (type == "main")
                {
                    _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
                }
                else if (type == "down")
                {
                    _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                }
                else if (type == "center")
                {
                    _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                }
                else if (type == "mobile")
                {
                    _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                }
                else if (type == "mobileblock")
                {
                    _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                }
                return Json(_IADManager.Delete(idlist, delaccount,this.LanguageID, account.Value, name));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region Save
        public ActionResult Save(ADEditModel model)
        {
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
                    var filename =  ticks + "." + fileformat.Last();
                    var adpath = "";
                    var imageadpath = "";
                    model.AD_Width = 0;
                    model.AD_Height = 600;
                    adpath = "ADMain";
                    imageadpath = "ad_main";

                    var checkpath = root + "\\UploadImage\\"+ adpath+"\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.Img_Name_Ori = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\"+ adpath+"\\" + model.Img_Name_Ori;
                    model.ImageFile.SaveAs(path);
                    model.Img_Show_Name = fullfilename;
                    var thumbpath = root + "\\UploadImage\\"+ adpath+"\\" + ticks + "_thumb." + fileformat.Last();
                    model.Img_Name_Thumb =  ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { imageadpath });
                    model.ImageFile.SaveAs(path);
                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, model.AD_Height.Value, fileformat.Last(),adjustImageEnum.limitMaxImageHeight);
                    if (haspath == "") { model.Img_Name_Thumb = ""; }

                    //表示有舊資料
                }

                if (model.UploadVideoFile != null)
                {
                    model.UploadVideoFileName = model.UploadVideoFile.FileName.Split('\\').Last();
                    var uploadfilepath = Request.PhysicalApplicationPath;
                    var newpath = uploadfilepath + "\\video\\ADEdit\\";
                    if (System.IO.Directory.Exists(newpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(newpath);
                    }
                    var guid = Guid.NewGuid();
                    var filename = DateTime.Now.Ticks + "." + model.UploadVideoFile.FileName.Split('.').Last();
                    var path = newpath + filename;
                    model.UploadVideoFilePath = "\\video\\ADEdit\\" + filename;
                    model.UploadVideoFile.SaveAs(path);
                }


                if (model.Type == "right")
                {
                    _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
                }
                else if (model.Type == "rightdown")
                {
                    _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                }
                else if (model.Type == "main")
                {
                    _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
                }
                else if (model.Type == "down")
                {
                    _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                }
                else if (model.Type == "center")
                {
                    _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                }
                else if (model.Type == "mobile")
                {
                    _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                }
                else if (model.Type == "mobileblock")
                {
                    _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                }
                model.Lang_ID =int.Parse( this.LanguageID);
                model.ADDesc = HttpUtility.UrlDecode(model.ADDesc);
                if (model.EdDateStr.IsNullorEmpty() == false) {
                    model.EdDate = DateTime.Parse(model.EdDateStr);
                }
                if (model.StDateStr.IsNullorEmpty() == false)
                {
                    model.StDate = DateTime.Parse(model.StDateStr);
                }
                if (model.ID >= 0)
                {
                    return Json(_IADManager.Update(model, this.Account, this.UserName));
                }
                else {
                    return Json(_IADManager.Create(model, this.Account, this.UserName));
                }
            }

            return Json("Edit");
        }
        #endregion

        #region SetStatus
        public ActionResult SetStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                if (type == "right")
                {
                    _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
                }
                else if (type == "rightdown")
                {
                    _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                }
                else if (type == "main")
                {
                    _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
                }
                else if (type == "down")
                {
                    _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                }
                else if (type == "center")
                {
                    _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                }
                else if (type == "mobile")
                {
                    _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                }
                else if (type == "mobileblock")
                {
                    _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                }
                return Json(_IADManager.UpdateStatus(id, status,this.Account,this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetStatus
        public ActionResult SetFixed(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                var user = Request.GetOwinContext().Authentication.User;
                var account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var name = user.Identity.Name;
                if (type == "right")
                {
                    _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
                }
                else if (type == "rightdown")
                {
                    _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
                }
                else if (type == "main")
                {
                    _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
                }
                else if (type == "down")
                {
                    _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
                }
                else if (type == "center")
                {
                    _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
                }
                else if (type == "mobile")
                {
                    _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
                }
                else if (type == "mobileblock")
                {
                    _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
                }
                return Json(_IADManager.UpdateFixed(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region VideoDownLoad
        public ActionResult VideoDownLoad(string id,string type)
        {
            if (type == "right")
            {
                _IADManager = new ADRightManager(new SQLRepository<ADRight>(connectionstr));
            }
            else if (type == "rightdown")
            {
                _IADManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
            }
            else if (type == "main")
            {
                _IADManager = new ADMainManager(new SQLRepository<ADMain>(connectionstr));
            }
            else if (type == "down")
            {
                _IADManager = new ADDownManager(new SQLRepository<ADDown>(connectionstr));
            }
            else if (type == "center")
            {
                _IADManager = new ADCenterManager(new SQLRepository<ADCenter>(connectionstr));
            }
            else if (type == "mobile")
            {
                _IADManager = new ADMobileManager(new SQLRepository<ADMobile>(connectionstr));
            }
            else if (type == "mobileblock")
            {
                _IADManager = new ADMobileBlockManager(new SQLRepository<ADMobileBlock>(connectionstr));
            }
            var    model = _IADManager.GetModel(id);
            string filepath = model.UploadVideoFilePath;
            string oldfilename = model.UploadVideoFileName;
            var uploadfilepath = Request.PhysicalApplicationPath;
            if (filepath != "")
            {
                //取得檔案名稱
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                //讀成串流
                Stream iStream = new FileStream(uploadfilepath + filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
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