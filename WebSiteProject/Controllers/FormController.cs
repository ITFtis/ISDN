using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;
using System.ServiceModel.Syndication;
using Utilities;
using System.Configuration;
using static WebSiteProject.FilterConfig;
using NAudio.Wave;
using NAudio.Lame;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace WebSiteProject.Controllers
{
    public class FormController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelFormManager _IModelFormManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        public FormController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelFormManager = serviceinstance.ModelFormManager;
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILoginManager = serviceinstance.LoginManager;
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (this.IsNojavascript) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
           
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null) {
                if (Session["messagemodelid"].ToString() == itemid) {
                    if (Session["messagepage"] != null)
                    {
                        ViewBag.nowpage = Session["messagepage"];
                    }
                    if (Session["messagegroup"] != null)
                    {
                        ViewBag.groupid = Session["messagegroup"];
                    }
                }
            }
            Session["messagepage"] = null;
            Session["messagegroup"] = null;
            Session["messagemodelid"] = null;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            var unitmodel = _IModelFormManager.GetUnitModel(itemid);
            FormFrontIndexModel model = new FormFrontIndexModel();
            _MasterPageManager.SetModel<FormFrontIndexModel>(ref model, Device, LangID, mid);
            if (model.IsNoJs == 1) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            var mainmodel = _IModelFormManager.Where(new ModelFormMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            if (mainmodel.First().IsVerift==false)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
          
            if (menu!=null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.MainID = itemid;
            model.MenuID= string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.Title = mainmodel.First().Name;
            var setting = _IModelFormManager.GetFormSetting(itemid);
            model.Desc = setting.FormDesc;
            var langdict = new Dictionary<string, string>();
            langdict.Add("mustinputstr", Common.GetLangText("必填"));
            langdict.Add("pleaseselectStr", Common.GetLangText("請選擇"));
            langdict.Add("checkcode", Common.GetLangText("驗證碼"));
            langdict.Add("checkcodeinput", Common.GetLangText("請輸入驗證碼"));
            langdict.Add("refresh", Common.GetLangText("重新整理"));
            langdict.Add("audioplay", Common.GetLangText("語音播放"));
            langdict.Add("CatchStr2", Common.GetLangText("驗證碼輸入錯誤"));
            var formdata = _IModelFormManager.GetFormList(itemid, langdict);
            model.Formhtml = formdata[0];
            model.CatchStr = formdata[1];
            return View(model);
        }
        #endregion

        #region IndexNoJs
        [NoCacheAttribute]
        public ActionResult IndexNoJs(string itemid, string mid,string formerror)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                _MasterPageManager.SetNoJS(0);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null)
            {
                if (Session["messagemodelid"].ToString() == itemid)
                {
                    if (Session["messagepage"] != null)
                    {
                        ViewBag.nowpage = Session["messagepage"];
                    }
                    if (Session["messagegroup"] != null)
                    {
                        ViewBag.groupid = Session["messagegroup"];
                    }
                }
            }
            Session["messagepage"] = null;
            Session["messagegroup"] = null;
            Session["messagemodelid"] = null;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            var unitmodel = _IModelFormManager.GetUnitModel(itemid);
            FormFrontIndexModel model = new FormFrontIndexModel();
            _MasterPageManager.SetModel<FormFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelFormManager.Where(new ModelFormMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                _MasterPageManager.SetNoJS(0);
                return RedirectToAction("Index", "Home");
            }
            if (mainmodel.First().IsVerift == false)
            {
                _MasterPageManager.SetNoJS(0);
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            if (menu != null)
            {
                _MasterPageManager.SetNoJS(0);
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.Title = mainmodel.First().Name;
            var setting = _IModelFormManager.GetFormSetting(itemid);
            model.Desc = setting.FormDesc;
            var langdict = new Dictionary<string, string>();
            langdict.Add("CatchStr2", Common.GetLangText("驗證碼輸入錯誤"));
            langdict.Add("Error2", Common.GetLangText("請確實輸入日期格式"));
            langdict.Add("Error3", Common.GetLangText("參觀日期必須為星期三"));
            langdict.Add("Error4", Common.GetLangText("參觀日期必須為一個月後"));
            langdict.Add("mustinputstr", Common.GetLangText("必填"));
            langdict.Add("pleaseselectStr", Common.GetLangText("請選擇"));
            langdict.Add("checkcode", Common.GetLangText("驗證碼"));
            langdict.Add("checkcodeinput", Common.GetLangText("請輸入驗證碼"));
            langdict.Add("refresh", Common.GetLangText("重新整理"));
            langdict.Add("audioplay", Common.GetLangText("語音播放"));
            var audiosrc = Url.Action("GetAudio");
            langdict.Add("getaudio", audiosrc);
            if (Session["FormError"] == null)
            {
                if (formerror != null) {
                    var erroritem = formerror.Split(',').ToList();
                    var formdata = _IModelFormManager.GetFormListNoJs(itemid, erroritem, langdict);
                    model.Formhtml = formdata[0].Replace("#ReCaptch", Url.Action("IndexNoJs", new { itemid = itemid, mid = mid }));
                    model.CatchStr = formdata[1];
                }
                else
                {
                    var formdata = _IModelFormManager.GetFormListNoJs(itemid, new List<string>(), langdict);
                    model.Formhtml = formdata[0].Replace("#ReCaptch", Url.Action("IndexNoJs", new { itemid = itemid, mid = mid }));
                    model.CatchStr = formdata[1];
                }
            }
            else {
                var erroritem = (List<string>)Session["FormError"];
                var formdata = _IModelFormManager.GetFormListNoJs(itemid, erroritem, langdict);
                model.Formhtml = formdata[0].Replace("#ReCaptch", Url.Action("IndexNoJs", new { itemid = itemid, mid = mid }));
                model.CatchStr = formdata[1];
                Session.Remove("FormError");
            }
            _MasterPageManager.SetNoJS(0);
            return View(model);
        }
        #endregion

        #region ConfirmContent
        public ActionResult ConfirmContent(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null)
            {
                if (Session["messagemodelid"].ToString() == itemid)
                {
                    if (Session["messagepage"] != null)
                    {
                        ViewBag.nowpage = Session["messagepage"];
                    }
                    if (Session["messagegroup"] != null)
                    {
                        ViewBag.groupid = Session["messagegroup"];
                    }
                }
            }
            Session["messagepage"] = null;
            Session["messagegroup"] = null;
            Session["messagemodelid"] = null;
            var unitmodel = _IModelFormManager.GetUnitModel(itemid);
            FormFrontIndexModel model = new FormFrontIndexModel();
            _MasterPageManager.SetModel<FormFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelFormManager.Where(new ModelFormMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            if (string.IsNullOrEmpty(mid) == false)
            {
                var menu = _IMenuManager.GetModel(mid);
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.Title = mainmodel.First().Name;
            var setting = _IModelFormManager.GetFormSetting(itemid);
            model.Desc = setting.ConfirmContent;
            return View(model);
        }
        #endregion

        #region GetFormList
        public ActionResult GetFormList(string itemid)
        {
            var langdict = new Dictionary<string, string>();
            langdict.Add("mustinputstr", Common.GetLangText("必填"));
            langdict.Add("pleaseselectStr", Common.GetLangText("請選擇"));
            langdict.Add("checkcode", Common.GetLangText("驗證碼"));
            langdict.Add("checkcodeinput", Common.GetLangText("請輸入驗證碼"));
            langdict.Add("refresh", Common.GetLangText("重新整理"));
            langdict.Add("audioplay", Common.GetLangText("語音播放"));
            return Json(_IModelFormManager.GetFormList(itemid, langdict));
        }
        #endregion

        #region SaveFormNoJs
        public ActionResult SaveFormNoJs(string jsonstr, string itemid,string mid,string CatchStr)
        {
            //if (Request.HttpMethod.ToLower() == "post")
            //{
            //    System.Web.Helpers.AntiForgery.Validate();
            //}
            var allform = Request.Form.AllKeys;
            var dict = new Dictionary<string, string>();
            var writeCatchStr = "";
            itemid = Request.Form["itemid"];

            var FormSelItems = _IModelFormManager.GetFormSelItemByItemID(itemid);
            FormSelItems = FormSelItems.Where(x => x.MustInput.Value).ToList();
            var err1idlist = new List<string>();
            foreach (var str in allform)
            {
                if (str.IndexOf("item_") >= 0)
                {
                    var key = str.Replace("item_", "");
                    var value = Request.Form[str];
                    dict.Add(key, value == null ? "" : value);
                }
                if (str== "img_captch")
                {
                    writeCatchStr = Request.Form[str];
                    if (writeCatchStr.IsNullorEmpty())
                    {
                        err1idlist.Add("CatchStr");
                    }
                    else if (writeCatchStr != CatchStr) {
                        err1idlist.Add("CatchStr2");
                    }
                }
             }

            foreach (var sel in FormSelItems) {
                if (dict.ContainsKey(sel.ID.ToString()) == false)
                {
                    err1idlist.Add(sel.ID.ToString());
                }
                else {
                    if (dict[sel.ID.ToString()].IsNullorEmpty())
                    {
                        err1idlist.Add(sel.ID.ToString());
                    }
                    else if (sel.Title == "參觀日期") {
                        var date = dict[sel.ID.ToString()];
                        DateTime vdate;
                        if (DateTime.TryParse(date, out vdate)==false) {
                            err1idlist.Add(sel.ID.ToString()+"Error2");
                        }else if (vdate.DayOfWeek!= DayOfWeek.Wednesday)
                        {
                            err1idlist.Add(sel.ID.ToString() + "Error3");
                        }
                        else if (DateTime.Now.AddMonths(1).AddDays(1)> vdate)
                        {
                            err1idlist.Add(sel.ID.ToString() + "Error4");
                        };

                    }
                }
            }
            if (err1idlist.Count() > 0)
            {
                Session["FormError"]= err1idlist;
                return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid, formerror=string.Join(",",err1idlist) });
            }
            else {
                jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(dict);
                var v = _IModelFormManager.SaveForm(jsonstr, itemid);
                return RedirectToAction("ConfirmContent", new { itemid = itemid, mid = mid });
            }

        }
        #endregion

        #region SaveForm
        public ActionResult SaveForm(string jsonstr, string itemid)
        {
            return Json(_IModelFormManager.SaveForm(jsonstr, itemid));

        }
        #endregion

        [AllowAnonymous]
        #region CaptchRefresh
        public ActionResult CaptchRefresh()
        {
            var imagestrArr = _ILoginManager.GetCaptchImage();
            Session["Captch"] = imagestrArr[0];
            return Json(new string[] { imagestrArr[0], imagestrArr[1] });
        }
        #endregion

        #region GetAudio
        public async Task<ActionResult> GetAudio(string text)
        {
            Task<FileContentResult> task = Task.Run(() =>
            {
                using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                {
                    MemoryStream stream = new MemoryStream();

                    speechSynthesizer.SetOutputToWaveStream(stream);
                    var textarr = text.ToArray();
                    foreach (var t in textarr)
                    {
                        speechSynthesizer.Speak(t.ToString());
                    }
                    var bytes = stream.GetBuffer();
                    var mp3bytes = ConvertWavStreamToMp3File(ref stream, Server.MapPath("/UploadImage/fileName.mp3"));
                    return File(mp3bytes, "audio/mpeg");
                }
            });
            return await task;
        }

        #endregion
        #region ConvertWavStreamToMp3File
        private byte[] ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
        {
            CheckAddBinPath();
            ms.Seek(0, SeekOrigin.Begin);
            MemoryStream msmp3 = new MemoryStream();
            using (var retMs = new MemoryStream())
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(msmp3, rdr.WaveFormat, LAMEPreset.VBR_90))
            {
                rdr.CopyTo(wtr);
            }
            return msmp3.ToArray();
        }
        public void CheckAddBinPath()
        {
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        } 
        #endregion
    }
}