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
using Utilities;
using ViewModels;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;
using NAudio.Wave;
using NAudio.Lame;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace WebSiteProject.Controllers
{
    public class PageController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelPageEditManager _IModelPageEditManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        public PageController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelPageEditManager = serviceinstance.ModelPageEditManager;
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILoginManager = serviceinstance.LoginManager;
        }

        #region Index
        public ActionResult Index(string itemid, string pageitemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid)) { return RedirectToAction("Index", "Home"); }
            int tempid = 0;
            if (int.TryParse(itemid, out tempid) == false)
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodelList = _IModelPageEditManager.GetModelItem(itemid);
            if (itemmodelList == null || itemmodelList.Count() == 0) { return RedirectToAction("Index", "Home"); }
            var mainmodel = _IModelPageEditManager.Where(new ModelPageEditMain() { ID = int.Parse(itemid) });
            if (mainmodel == null || mainmodel.Count() == 0) { return RedirectToAction("Index", "Home"); }
            var unitmodel = _IModelPageEditManager.GetUnitModel(itemid);
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            PageFrontIndexModel model = new PageFrontIndexModel();
            _MasterPageManager.SetModel<PageFrontIndexModel>(ref model, Device, LangID, mid);
            model.MenuType = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.SiteMenuID = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.ModelItemList = _IModelPageEditManager.GetSelectList(itemid);
            PageIndexItem itemmodel = new PageIndexItem();
            model.ItemCnt = itemmodelList.Count();

            ViewBag.pageitemid = "";
            if (itemmodelList.Count() > 0)
            {
                if (pageitemid.IsNullorEmpty())
                {
                    itemmodel = itemmodelList.First();
                    model.PageItemID = itemmodelList.First().ItemID.Value;
                }
                else
                {
                    if (int.TryParse(pageitemid, out tempid) == false)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    model.PageItemID = int.Parse(pageitemid);
                    var tempobj = itemmodelList.Where(v => v.ItemID == int.Parse(pageitemid));
                    if (tempobj.Count() > 0) { itemmodel = tempobj.First(); }
                }
            }
            if (itemmodel.ModelID == null) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.IsVerift == false) { return RedirectToAction("Index", "Home"); }
            if (menu != null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName == null ? "" : menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            if (itemmodel.ImageFileName.IsNullorEmpty() == false)
            {
                itemmodel.ImageFileName = helper.Content("~/UploadImage/PageEdit/" + itemmodel.ImageFileName);
                var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = itemmodel.ImageFileName, Query = null, };
                model.FBImage = urlBuilder.ToString();
            }
            else
            {
                //UploadImage/PageEdit
                if (itemmodel.HtmlContent.IndexOf("UploadImage/PageEdit") >= 0)
                {
                    model.FBImage = "";
                }
                else
                {
                    var ImageFileName = helper.Content("~/img/logo_fb.jpg");
                    var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = ImageFileName, Query = null, };
                    model.FBImage = urlBuilder.ToString();
                }
            }
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title, true);
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, model.SiteMenuID);
            model.HtmlContent = itemmodel.HtmlContent == null ? "<div></div>" :
            itemmodel.HtmlContent
            .Replace("\n", "<br>")
            .Replace("<<br>", "<\n")
            .Replace("><br>", ">\n")
            .Replace("\n", "")
            .Replace("\t", "")

            // 解決W3C問題
            .Replace("<font", "<b")
            .Replace("/font", "b")

            // 解決開源格式問題
            .Replace(".docx", ".odt")
            .Replace(".doc", ".odt");

            // 轉換成html格式
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.LoadHtml(model.HtmlContent);

            // 針對 img 去修正
            var imageNodes = xdoc.DocumentNode.Descendants("img").ToList();
            for (int index = 0; index < imageNodes.Count; index++)
            {
                var node = imageNodes[index];
                if (!node.Attributes.Where(e => e.Name == "alt").Any())
                {
                    //node.Remove(); //為了解決無障礙問題
                    //node.SetAttributeValue("alt", MessageController.getName());
                }
            }

            // 針對table的修正
            var tableNodes = xdoc.DocumentNode.Descendants("table");
            foreach (var table in tableNodes)
            {

                //判斷是否有header
                if (!table.Descendants("thead").Any())
                {
                    var body = table.Descendants("tbody").FirstOrDefault();
                    if (body != null)
                    {
                        var head = HtmlNode.CreateNode("<thead></thead>");

                        var firstRow = body.Descendants("tr").FirstOrDefault();
                        head.AppendChild(firstRow); // 複製第一row到head去
                        body.RemoveChild(firstRow); // 移除第一row

                        table.PrependChild(head);
                    }
                }

                // 替換head內的標籤
                if (table.Descendants("thead").Any())
                {
                    var head = table.Descendants("thead").FirstOrDefault();
                    var headString = head.OuterHtml.Replace("<td", "<th").Replace("</td>", "</th>");
                    table.RemoveChild(head);

                    var newHead = HtmlNode.CreateNode(headString);
                    table.PrependChild(newHead);
                }
            }
            model.HtmlContent = xdoc.DocumentNode.OuterHtml;



            var fbtitle = model.HtmlContent.TrimgHtmlTag();
            model.FBTitle = fbtitle.Count() > 80 ? fbtitle.Substring(0, 80) : fbtitle;
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.ItemID = itemmodel.ItemID.ToString();
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.UploadFileName = itemmodel.UploadFileName;
            model.ItemName = itemmodel.ItemName;

            if (itemmodel.LinkUrl.IsNullorEmpty() == false)
            {
                model.LinkUrl = itemmodel.LinkUrl;
            }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            return View(model);
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string itemid)
        {
            var model = _IModelPageEditManager.GetlPageItem(itemid);
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

        #region Forward
        public ActionResult Forward(string itemid, string mid, string pageitemid, string finfo)
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
            var unitmodel = _IModelPageEditManager.GetUnitModel(itemid);
            var langid = _MasterPageManager.CheckLangID(mid);
            var sitemodel = _ISiteLayoutManager.GetSiteLayout(Device, langid);
            ViewBag.FooterString = sitemodel.FowardHtmlContent;
            ViewBag.LogoUrl = sitemodel.FowardImageUrl;
            ViewBag.itemid = itemid;
            ViewBag.mid = mid;
            ViewBag.pageitemid = pageitemid;

            var mainmodel = _IModelPageEditManager.Where(new ModelPageEditMain() { ID = int.Parse(itemid) });
            if (mainmodel == null) { return RedirectToAction("Index", "Home"); }

            var itemmodel = _IModelPageEditManager.GetlPageItem(itemid);
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            if (menu != null)
            {
                ViewBag.Title = menu.MenuName;
            }
            else
            {
                ViewBag.Title = itemmodel.ItemName;
            }
            var hostUrl = string.Format("{0}://{1}",
              Request.Url.Scheme,
              Request.Url.Authority);
            if (string.IsNullOrEmpty(mid) == false && string.IsNullOrEmpty(pageitemid) == false)
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Page", new { itemid = itemid, mid = mid, pageitemid = pageitemid });
            }
            else if (string.IsNullOrEmpty(mid) == false && string.IsNullOrEmpty(pageitemid))
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Page", new { itemid = itemid, mid = mid });
            }
            else
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Page", new { itemid = itemid });
            }
            var imagestrArr = _ILoginManager.GetCaptchImage();
            ViewBag.audio = Url.Action("GetAudio");
            ViewBag.captch = imagestrArr[0];
            ViewBag.captchimg = imagestrArr[1];
            return View();
        }
        #endregion

        #region Print
        public ActionResult Print(string itemid, string pageitemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid)) { return RedirectToAction("Index", "Home"); }
            int tempid = 0;
            if (int.TryParse(itemid, out tempid) == false)
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodel = _IModelPageEditManager.GetlPageItem(itemid);
            var mainmodel = _IModelPageEditManager.Where(new ModelPageEditMain() { ID = itemmodel.ModelID.Value });
            if (mainmodel == null || mainmodel.Count() == 0) { return RedirectToAction("Index", "Home"); }
            var unitmodel = _IModelPageEditManager.GetUnitModel(itemid);
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            PageFrontIndexModel model = new PageFrontIndexModel();
            _MasterPageManager.SetModel<PageFrontIndexModel>(ref model, Device, LangID, mid);
            model.MenuType = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.SiteMenuID = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.ModelItemList = _IModelPageEditManager.GetSelectList(itemmodel.ModelID.Value.ToString());
            model.ItemCnt = 1;
            ViewBag.pageitemid = "";

            if (itemmodel.ModelID == null) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (menu != null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                model.Title = itemmodel.ItemName;
            }

            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title, true);
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, model.SiteMenuID);
            model.HtmlContent = itemmodel.HtmlContent == null ? "<div></div>" :
          itemmodel.HtmlContent
          .Replace("\n", "<br>")
          .Replace("<<br>", "<\n")
          .Replace("><br>", ">\n")
          .Replace("\n", "")
          .Replace("\t", "")

          // 解決W3C問題
          .Replace("<font", "<b")
          .Replace("/font", "b")

          // 解決開源格式問題
          .Replace(".docx", ".odt")
          .Replace(".doc", ".odt");

            // 轉換成html格式
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.LoadHtml(model.HtmlContent);

            // 針對 img 去修正
            var imageNodes = xdoc.DocumentNode.Descendants("img").ToList();
            for (int index = 0; index < imageNodes.Count; index++)
            {
                var node = imageNodes[index];
                if (!node.Attributes.Where(e => e.Name == "alt").Any())
                {
                    //node.Remove(); //為了解決無障礙問題
                    //node.SetAttributeValue("alt", MessageController.getName());
                }
            }

            // 針對table的修正
            var tableNodes = xdoc.DocumentNode.Descendants("table");
            foreach (var table in tableNodes)
            {

                //判斷是否有header
                if (!table.Descendants("thead").Any())
                {
                    var body = table.Descendants("tbody").FirstOrDefault();
                    if (body != null)
                    {
                        var head = HtmlNode.CreateNode("<thead></thead>");

                        var firstRow = body.Descendants("tr").FirstOrDefault();
                        head.AppendChild(firstRow); // 複製第一row到head去
                        body.RemoveChild(firstRow); // 移除第一row

                        table.PrependChild(head);
                    }
                }

                // 替換head內的標籤
                if (table.Descendants("thead").Any())
                {
                    var head = table.Descendants("thead").FirstOrDefault();
                    var headString = head.OuterHtml.Replace("<td", "<th").Replace("</td>", "</th>");
                    table.RemoveChild(head);

                    var newHead = HtmlNode.CreateNode(headString);
                    table.PrependChild(newHead);
                }
            }
            model.HtmlContent = xdoc.DocumentNode.OuterHtml;
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            model.UploadFileName = itemmodel.UploadFileName;
            model.ItemName = itemmodel.ItemName;
            if (itemmodel.LinkUrl.IsNullorEmpty() == false)
            {
                model.LinkUrl = itemmodel.LinkUrl;
            }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            return View(model);
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
    }
}