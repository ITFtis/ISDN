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
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using NAudio.Wave;
using NAudio.Lame;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using Newtonsoft.Json;
namespace WebSiteProject.Controllers
{
    public class DownloadController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelFileDownloadManager _IModelFileDownloadManager;
        IMenuManager _IMenuManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        public DownloadController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IMenuManager = serviceinstance.MenuManager;
            _IModelFileDownloadManager = serviceinstance.ModelFileDownloadManager;
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
            var unitmodel = _IModelFileDownloadManager.GetUnitModel(itemid);

            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            FileDownloadFrontIndexModel model = new FileDownloadFrontIndexModel();
            _MasterPageManager.SetModel<FileDownloadFrontIndexModel>(ref model, Device, LangID, mid);
            if (model.IsNoJs == 1) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            var mainmodel = _IModelFileDownloadManager.Where(new ModelFileDownloadMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.MenuType = menutype;
            model.SiteMenuID = sitemenuid;
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
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList= _IModelFileDownloadManager.GetFrontGroupSelectList(itemid);
            model.GroupList.First().Text= Common.GetLangText("全部");
            model.Hasgroup= model.GroupList.Count() == 1 ? false : true;
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID= string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;

            return View(model);
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(FileDownloadSearchModel model)
        {
            var data = _IModelFileDownloadManager.PagingItemForWebSite(model.ModelID.ToString(), model, "");
            var allfile = _IModelFileDownloadManager.GetAllFileDownloadFiles(model.ModelID.ToString());
            var unitmodel = _IModelFileDownloadManager.GetUnitModel(model.ModelID.ToString());
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/img/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
           var isshowpublic = 1;
            if (ColumnSetting != null)
            {
                var ispublic = ColumnSetting.Where(v => v.Name == "發佈日期");
                if (ispublic.Count() > 0)
                {
                    isshowpublic = ispublic.First().Sellected;
                }
            }
            foreach (var _d in data.rows) {
                sb.Append("<div class='item' data-sr='enter bottom over 1.5s'>");
                if (_d.RelatceImageFileName != "")
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/FileDownloadItem/" + _d.RelatceImageFileName) + "'  alt='" + _d.Title + "'>");
                }
                else
                {
                    sb.Append("<img src = '" + baseimg + "' alt='" + _d.Title + "'>");
                }
                var group = _d.GroupName.IsNullorEmpty() ? "" : "<div class='top_class'>" + _d.GroupName +"</div>";
                if (isshowpublic == 1)
                {
                    sb.Append("<div class='date'>" + _d.PublicshDate + group + "</div>");
                }
                else {
                    sb.Append("<div class='date'>"  + group + "</div>");
                }
                sb.Append("<div class='title'>" + _d.Title + "</div>");
                sb.Append("<div class='con_font'>" + _d.HtmlContent + "</div>");
                sb.Append("<div class='link'>");
                var file = allfile.Where(v => v.ItemID == _d.ItemID);
                foreach (var f in file) {
                    sb.Append("<a href = '"+ Url.Action("FileDownLoad", new { fileid =f.ID }) +"' title='檔案下載-" + _d.Title + ".pdf(另開新視窗)' target='_blank'>");
                    sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true'><i class='fa fa-square fa-stack-2x font-blue-steel'  aria-hidden='true'></i><i class='fas fa-download fa-stack-1x'  aria-hidden='true'></i><span class='sr-only'>檔案下載</span></span></a>");
                }
                sb.Append("</div><div class='text-shadow-container'></div></div>");
                
            }
            decimal pagecnt = -1;
            if (model.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)model.Limit);
            }
            return Json(new string[] { sb.ToString(), data.total.ToString(), pagecnt.ToString() });
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string fileid)
        {
            var model = _IModelFileDownloadManager.GetFileDownloadFiles(fileid);
            string filepath = "";
            string oldfilename = "";
            _IModelFileDownloadManager.UpdateClickCount(model.ItemID.ToString());
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

        #region Forward
        public ActionResult Forward(string itemid, string mid, string menutype, string sitemenuid, string finfo)
        {
            if (finfo.IsNullorEmpty() == false)
            {
                var focusid = "";
                var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(finfo);
                if (info.ContainsKey("result")) { ViewBag.result = info["result"]; }
                if (info.ContainsKey("Sender")) { ViewBag.SenderError = "Y"; if (focusid == "") { focusid = "Sender"; } }
                if (info.ContainsKey("SenderEMail")) { ViewBag.SenderEMailError = "Y"; if (focusid == "") { focusid = "SenderEMail"; } }
                if (info.ContainsKey("SenderEMailFormat")) { ViewBag.SenderEMailFormatError = "Y"; if (focusid == "") { focusid = "SenderEMail"; } }
                if (info.ContainsKey("ForwardEMail")) { ViewBag.ForwardEMailError = "Y"; if (focusid == "") { focusid = "ForwardEMail"; } }
                if (info.ContainsKey("ForwardEMailFormat")) { ViewBag.ForwardEMailFormatError = "Y"; if (focusid == "") { focusid = "ForwardEMail"; } }

                if (info.ContainsKey("CaptchError")) { ViewBag.CaptchError = "Y"; if (focusid == "") { focusid = "img_captch"; } }
                if (info.ContainsKey("CaptchInputError")) { ViewBag.CaptchInputError = "Y"; if (focusid == "") { focusid = "img_captch"; } }
                if (info.ContainsKey("errorinfo")) { ViewBag.errorinfo = info["errorinfo"]; }
                if (info.ContainsKey("sendstr"))
                {
                    var sendstr = info["sendstr"].Split('^');
                    if (sendstr.Length > 3)
                    {
                        ViewBag.Sender = sendstr[0];
                        ViewBag.SenderEMail = sendstr[1];
                        ViewBag.ForwardEMail = sendstr[2];
                        ViewBag.ForwardMessage = sendstr[3];
                    }

                }
                ViewBag.FocusID = focusid;
            }
            Session.Remove("ForwardInfo");
            ViewBag.itemid = itemid;
            ViewBag.mid = mid;
            ViewBag.menutype = menutype;
            ViewBag.sitemenuid = sitemenuid;
            var unitmodel = _IModelFileDownloadManager.GetUnitModel(itemid);
            var sitemodel = _ISiteLayoutManager.GetSiteLayout(Device, "1");
            ViewBag.FooterString = sitemodel.FowardHtmlContent;
            ViewBag.LogoUrl = sitemodel.FowardImageUrl;
            var mainmodel = _IModelFileDownloadManager.Where(new ModelFileDownloadMain() { ID = int.Parse(itemid) });
            if (mainmodel == null) { return RedirectToAction("Index", "Home"); }
            if (mainmodel.Count() > 0)
            {
                ViewBag.Title = mainmodel.First().Name;
            }

            var hostUrl = string.Format("{0}://{1}",
              Request.Url.Scheme,
              Request.Url.Authority);

            if (sitemenuid != "-1")
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Download", new { itemid = itemid, mid = mid, sitemenuid = sitemenuid, menutype = menutype });
            }
            else if (string.IsNullOrEmpty(mid) == false)
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Download", new { itemid = itemid, mid = mid });
            }
            else
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Download", new { itemid = itemid });
            }
            var imagestrArr = _ILoginManager.GetCaptchImage();
            ViewBag.audio = Url.Action("GetAudio");
            ViewBag.captch = imagestrArr[0];
            ViewBag.captchimg = imagestrArr[1];
            return View();
        }
        #endregion

        #region SendMail
        public ActionResult SendMail(string Sender, string SenderEMail, string ForwardEMail, string ForwardMessage, string Url, string Title,
            string itemid, string mid, string menutype, string sitemenuid, string captch, string img_captch)
        {
            var cancel = Request.Form["cancel"];
            var info = new Dictionary<string, string>();
            if (cancel == null)
            {

                try
                {
                    var echeck = new EmailAddressAttribute();
                    var sendstr = new List<string>();
                    sendstr.Add(Sender == null ? "" : Sender);
                    sendstr.Add(SenderEMail == null ? "" : SenderEMail);
                    sendstr.Add(ForwardEMail == null ? "" : ForwardEMail);
                    sendstr.Add(ForwardMessage == null ? "" : ForwardMessage);
                    if (Sender.IsNullorEmpty())
                    {
                        info.Add("Sender", "");
                    }
                    if (SenderEMail.IsNullorEmpty())
                    {
                        info.Add("SenderEMail", "");
                    }
                    else
                    {
                        if (echeck.IsValid(SenderEMail) == false)
                        {
                            info.Add("SenderEMailFormat", "");
                        }
                    }
                    if (ForwardEMail.IsNullorEmpty())
                    {
                        info.Add("ForwardEMail", "");
                    }
                    else
                    {
                        var fsplit = ForwardEMail.Split(',');
                        foreach (var v in fsplit)
                        {
                            if (echeck.IsValid(v) == false)
                            {
                                if (v == "") { continue; }
                                info.Add("ForwardEMailFormat", "");
                            }
                        }
                    }
                    if (img_captch.IsNullorEmpty())
                    {
                        info.Add("CaptchError", "");
                    }
                    else
                    {
                        if (img_captch != captch)
                        {
                            info.Add("CaptchInputError", "");
                        }
                    }
                    if (info.Count() == 0)
                    {
                        var host = System.Web.Configuration.WebConfigurationManager.AppSettings["smtphost"];
                        var mailfrom = System.Web.Configuration.WebConfigurationManager.AppSettings["mailfrom"];
                        var NoticeSenderEMail = mailfrom;
                        var NoticeSubject = Title;
                        var slist = ForwardEMail.Split(';');
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(SenderEMail, Sender);
                        foreach (var sender in slist)
                        {
                            message.To.Add(new MailAddress(sender));
                        }
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.Subject = NoticeSubject;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        string body = Sender + Common.GetLangText("寄了一則訊息給你喔") + "<br/> " + Common.GetLangText("給您的訊息") + ":" + ForwardMessage +
                            "<br/>" + Url;
                        message.Body = body;
                        message.IsBodyHtml = true;
                        message.Priority = MailPriority.High;
                        var ur = System.Web.Configuration.WebConfigurationManager.AppSettings["mailuser"];
                        var pw = System.Web.Configuration.WebConfigurationManager.AppSettings["mailpassword"];
                        var port = System.Web.Configuration.WebConfigurationManager.AppSettings["mailport"];
                        var rmessage = "";
                        try
                        {
                            if (string.IsNullOrEmpty(pw) == false)
                            {
                                SmtpClient client = new SmtpClient(host, int.Parse(port));
                                client.EnableSsl = true;
                                client.Credentials = new NetworkCredential(ur, pw);
                                client.Send(message);
                            }
                            else
                            {
                                SmtpClient client2 = new SmtpClient(host);
                                client2.Send(message);
                            }
                            rmessage = "轉寄好友寄送成功";
                        }
                        catch (Exception ex)
                        {
                            rmessage = "轉寄好友寄送失敗:" + ex.Message;
                        }
                        return RedirectToAction("ForwardResult", "Home", new { title = Title, message = rmessage });
                    }
                    else
                    {
                        info.Add("result", "error"); info.Add("sendstr", string.Join("^", sendstr));
                    }
                }
                catch (Exception ex)
                {
                    info.Add("result", "exception");
                    info.Add("errorinfo", "寄信失敗:" + ex.Message);
                }
            }
            return RedirectToAction("Forward", new { itemid = itemid, mid = mid, menutype = menutype, sitemenuid = sitemenuid, finfo = JsonConvert.SerializeObject(info) });
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

        #region Print
        public ActionResult Print(string id, string mid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            FileDownloadFrontIndexModel model = new FileDownloadFrontIndexModel();
            _MasterPageManager.SetModel<FileDownloadFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelFileDownloadManager.Where(new ModelFileDownloadMain() { ID = int.Parse(id) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.MenuType = menutype;
            model.SiteMenuID = sitemenuid;
            if (menu != null)
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
            var unitmodel = _IModelFileDownloadManager.GetUnitModel(id);
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IModelFileDownloadManager.GetFrontGroupSelectList(id);
            model.GroupList.First().Text = Common.GetLangText("全部");
            model.Hasgroup = model.GroupList.Count() == 1 ? false : true;
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            return View(model);
        }
        #endregion

        #region IndexNoJs
        [NoCacheAttribute]

        public ActionResult IndexNoJs(string itemid, string mid)
        {
            var btntype = Request["btntype"] == null ? "news_list" : Request["btntype"];
            var pagetype = Request["pagetype"].IsNullorEmpty() ? "news_list" : Request["pagetype"];
            var GroupId = "";
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (btntype == "search")
            {
                GroupId = Request["GroupId"].IsNullorEmpty() ? "" : Request["GroupId"];
                DisplayFrom = Request["DisplayFrom"].IsNullorEmpty() ? "" : Request["DisplayFrom"];
                DisplayTo = Request["DisplayTo"].IsNullorEmpty() ? "" : Request["DisplayTo"];
                keyword = Request["keyword"].IsNullorEmpty() ? "" : Request["keyword"];
                var skey = new Dictionary<string, string>();
                if (GroupId != "" || DisplayFrom != "" || DisplayTo != "" || keyword != "")
                {
                    skey.Add("GroupId", GroupId);
                    skey.Add("DisplayFrom", DisplayFrom);
                    skey.Add("DisplayTo", DisplayTo);
                    skey.Add("keyword", keyword);
                    Session["DownloadIndexNoJsSearch"] = skey;
                }
                else
                {
                    if (Session["DownloadIndexNoJsSearch"] != null) { Session.Remove("DownloadIndexNoJsSearch"); }
                }
            }
            else if (btntype.IsNullorEmpty() == false)
            {
                pagetype = btntype;
            }
            ViewBag.pagetype = pagetype;
            var page_list = Request["page_list"].IsNullorEmpty() ? "" : Request["page_list"];
            var pindex = Request["pindex"].IsNullorEmpty() ? "1" : Request["pindex"];
            var maxpage = Request["maxpage"].IsNullorEmpty() ? "999" : Request["maxpage"];

            if (Session["DownloadIndexNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["DownloadIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("DownloadIndexNoJsSearch");
                    }
                }

            }
            ViewBag.GroupId = GroupId;
            ViewBag.DisplayFrom = DisplayFrom;
            ViewBag.DisplayTo = DisplayTo;
            ViewBag.keyword = keyword;

            int nowpage = 1;
            try
            {
                nowpage = Request["nowpage"] == null ? 1 : int.Parse(Request["nowpage"]);
            }
            catch (Exception ex)
            {
                nowpage = 1;
            }

            if (pindex == "1")
            {
                nowpage = 1;
            }
            else if (pindex == "-1")
            {
                nowpage -= 1;
            }
            else if (pindex == "+1")
            {
                nowpage += 1;
            }
            else
            {
                try
                {
                    nowpage = int.Parse(pindex);
                }
                catch (Exception ex)
                {
                    nowpage = 1;
                }
            }
            if (page_list != "")
            {
                try
                {
                    nowpage = int.Parse(page_list);
                }
                catch (Exception ex)
                {
                    nowpage = 1;
                }
            }
            if (nowpage <= 0) { nowpage = 1; }
            try
            {
                var maxpagecnt = int.Parse(maxpage);
                if (nowpage > maxpagecnt) { nowpage = maxpagecnt; }
            }
            catch (Exception ex)
            {
                nowpage = 1;
            }

            var page = nowpage;
            //if(pindex)
            ViewBag.nowpage = page;
            if (string.IsNullOrEmpty(itemid))
            {
                _MasterPageManager.SetNoJS(0);
                return RedirectToAction("Index", "Home");
            }

            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            var unitmodel = _IModelFileDownloadManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            FileDownloadFrontIndexModel model = new FileDownloadFrontIndexModel();
            _MasterPageManager.SetModel<FileDownloadFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelFileDownloadManager.Where(new  ModelFileDownloadMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
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
            model.GroupList = _IModelFileDownloadManager.GetGroupSelectList(itemid, false);
            model.Hasgroup = model.GroupList.Count() == 1 ? false : true;
            model.GroupList.First().Text = Common.GetLangText("全部");
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            var limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value;
            var offset = ((page - 1) * limit);
            var searchmodel = new FileDownloadSearchModel()
            {
                Sort = "Sort",
                Search = "Y",
                Limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value,
                ModelID = int.Parse(model.MainID),
                MenuId = model.MenuID,
                Offset = offset,
                GroupId = GroupId == "" ? (int?)null : int.Parse(GroupId),
                DisplayFrom = DisplayFrom,
                DisplayTo = DisplayTo,
                Title = keyword
            };
            var data = _IModelFileDownloadManager.PagingItemForWebSite(searchmodel.ModelID.ToString(), searchmodel, "");
            var allfile = _IModelFileDownloadManager.GetAllFileDownloadFiles(searchmodel.ModelID.ToString());
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            var isshowpublic = 1;
            foreach (var _d in data.rows)
            {
                sb.Append("<div class='item' data-sr='enter bottom over 1.5s'>");
                if (_d.RelatceImageFileName != "")
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/FileDownloadItem/" + _d.RelatceImageFileName) + "'  alt='" + _d.Title + "'>");
                }
                else
                {
                    sb.Append("<img src = '" + baseimg + "' alt='" + _d.Title + "'>");
                }
                var group = _d.GroupName.IsNullorEmpty() ? "" : "<div class='top_class'>" + _d.GroupName + "</div>";
                if (isshowpublic == 1)
                {
                    sb.Append("<div class='date'>" + _d.PublicshDate + group + "</div>");
                }
                else
                {
                    sb.Append("<div class='date'>" + group + "</div>");
                }
                sb.Append("<div class='title'>" + _d.Title + "</div>");
                sb.Append("<div class='con_font'>" + _d.HtmlContent + "</div>");
                sb.Append("<div class='link'>");
                var file = allfile.Where(v => v.ItemID == _d.ItemID);
                foreach (var f in file)
                {
                    sb.Append("<a href = '" + Url.Action("FileDownLoad", new { fileid = f.ID }) + "' title='檔案下載-" + _d.Title + ".pdf(另開新視窗)' target='_blank'>");
                    sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true'><i class='fa fa-square fa-stack-2x font-blue-steel'  aria-hidden='true'></i><i class='fas fa-download fa-stack-1x'  aria-hidden='true'></i><span class='sr-only'>檔案下載</span></span></a>");
                }
                sb.Append("</div><div class='text-shadow-container'></div></div>");

            }

            decimal pagecnt = -1;
            if (searchmodel.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)searchmodel.Limit);
            }
            ViewBag.Html = sb.ToString();

            var endcnt = (searchmodel.Offset + searchmodel.Limit) > data.total ? data.total : (searchmodel.Offset + searchmodel.Limit);
            ViewBag.TotalCntStr = data.total + "， " + Common.GetLangText("顯示") + " : " + (searchmodel.Offset + 1) + "~" + endcnt;
            ViewBag.maxpage = 0;
            if (data.total == 0 || unitmodel.ShowCount == -1) { ViewBag.showpagenum = "N"; }
            else
            {
                ViewBag.maxpage = pagecnt;
            }
            _MasterPageManager.SetNoJS(0);
            return View(model);
        }
        #endregion
    }
}