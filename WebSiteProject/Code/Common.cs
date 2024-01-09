using SQLModel;
using SQLModel.Models;
using System;
using System.Web;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Utilities;
using System.Web.Mvc;

namespace WebSiteProject.Code
{
    public static class Common
    {
        public static  string connectionStr= System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionStr"].ConnectionString;
       // public static string connectionMemberStr = System.Configuration.ConfigurationManager.ConnectionStrings["TPCAMember"].ConnectionString; 
        //public static string websitytype = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["websitetype"]) ? "P" : System.Configuration.ConfigurationManager.AppSettings["websitetype"];
        private static SQLRepository<SystemRecord> _systemrecordrepository = new SQLRepository<SystemRecord>(connectionStr);
        public static Dictionary<string, Dictionary<string, string>> LangDict = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, string> TCHNLang = new Dictionary<string, string>();
        public static Dictionary<string, string> SCHNLang = new Dictionary<string, string>();
        public static Dictionary<string, string> EngLang = new Dictionary<string, string>();
        public static string useencryptid = System.Configuration.ConfigurationManager.AppSettings["useencryptid"] == null?"N": System.Configuration.ConfigurationManager.AppSettings["useencryptid"];
        public static string recaptchakeyv2 = System.Configuration.ConfigurationManager.AppSettings["recaptcha_key_v2"];
        
        #region CreateLogin
        public static void CreateLogin(int userid, string account)
        {
            try
            {
                _systemrecordrepository.DelDataUseWhere("Login<@1", new object[] { DateTime.Now.AddDays(-120) });
                string tClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                var HTTP_VIA = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"];
                if (string.IsNullOrEmpty(HTTP_VIA) == false)
                {
                    var viaip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(viaip) == false)
                    {
                        tClientIP = viaip;
                    }
                }
                var nowdatetime = DateTime.Now;
                var model = new SystemRecord()
                {
                    Account = account,
                    Logs = nowdatetime.ToString("yyyy/MM/dd HH:mm:ss") + "登入系統",
                    IP = tClientIP,
                    Login = nowdatetime,
                    UserID = userid
                };
                var r = _systemrecordrepository.Create(model);
                if (r > 0)
                {
                    HttpContext.Current.Session["LoginDateTime"] = nowdatetime.Ticks;
                }
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region SetLogout
        public static void SetLogout(string userid)
        {
            try
            {
                var logindate = HttpContext.Current.Session["LoginDateTime"];
                if (logindate != null)
                {
                    var datetimesttr = new DateTime(long.Parse(logindate.ToString()));
                    var data = _systemrecordrepository.GetByWhere("Login=@1 and UserID=@2", new object[] { datetimesttr, userid });
                    if (data.Count() > 0)
                    {
                        var info = data.First().Logs + Environment.NewLine + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "登出系統";
                        _systemrecordrepository.Update("Logout=@1,Logs=@2", "Login=@3 and UserID=@4", new object[] { DateTime.Now, info, datetimesttr, userid });
                    }
                    else
                    {
                        _systemrecordrepository.Update("Logout=@1", "Login=@2 and UserID=@3", new object[] { DateTime.Now, datetimesttr, userid });
                    }

                }
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region SetLogs
        public static void SetLogs(string userid, string account, string log)
        {
            try
            {
                var logindate = HttpContext.Current.Session["LoginDateTime"];
                if (logindate == null)
                {
                    CreateLogin(int.Parse(userid), account);
                    logindate = HttpContext.Current.Session["LoginDateTime"];
                }
                if (logindate != null)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DoThreadLog), new object[] { logindate, userid , log });
                }
            }
            catch (Exception)
            {

            }

        } 
        #endregion

        #region DoThreadLog
        public static void DoThreadLog(object dataarray)
        {
            try
            {
                object[] objarr = (object[])dataarray;
                var datetimesttr = new DateTime(long.Parse(objarr[0].ToString()));
                var data = _systemrecordrepository.GetByWhere("Login=@1 and UserID=@2", new object[] { datetimesttr, objarr[1].ToString() });
                var info = data.First().Logs + Environment.NewLine + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + objarr[2].ToString();
                _systemrecordrepository.Update("Logs=@1", "Login=@2 and UserID=@3", new object[] { info, datetimesttr, objarr[1].ToString() });

            }
            catch (Exception )
            {
            }
        }
        #endregion

        #region GetLayout
        public static string GetLayout(int type,string langid)
        {
            return "~/Views/Shared/_Layout.cshtml";
            //if (type == 1) {
            //    return "~/Views/Shared/_LayoutIn.cshtml"; }
            //else
            //{
            //    return "~/Views/Shared/_LayoutIn.cshtml";
            //    //return "~/Views/Shared/_Layout2.cshtml";
            //}
        }
        #endregion

        #region GetIndexLayout
        public static string GetIndexLayout( string langid)
        {
            return "~/Views/Shared/_Layout.cshtml";
            //if (langid == "1")
            //{
            //    return "~/Views/Shared/_Layout.cshtml";
            //}
            //else
            //{
            //    return "~/Views/Shared/_Layout_en.cshtml";
            //}
        }
        #endregion

        #region SetAllLangKey
        public static void SetAllLangKey()
        {
            SQLRepository<LangKey> _langkeysqlrepository = new SQLRepository<LangKey>(connectionStr);
            SQLRepository<LangInputText> _langinputsqlrepository = new SQLRepository<LangInputText>(connectionStr);
            SQLRepository<Lang> _langsqlrepository = new SQLRepository<Lang>(connectionStr);
            var allkey = _langkeysqlrepository.GetAll();
            var alltext= _langinputsqlrepository.GetAll();
            TCHNLang.Clear();
            SCHNLang.Clear();
            EngLang.Clear();
            var ldata = _langsqlrepository.GetByWhere("Deleted=0 and Enabled=1", null);
            var tchidx = 1;
            if (ldata.Any(v => v.Lang_Name == "繁體中文")) { tchidx = ldata.Where(v => v.Lang_Name == "繁體中文").First().ID.Value; }
            var engidx =2;
            if (ldata.Any(v => v.Lang_Name == "英文")) { engidx = ldata.Where(v => v.Lang_Name == "英文").First().ID.Value; }
            var cchidx = 3;
            if (ldata.Any(v => v.Lang_Name == "簡體中文")) { cchidx = ldata.Where(v => v.Lang_Name == "簡體中文").First().ID.Value; }

            var tchntext = alltext.Where(v => v.LangID == tchidx).ToArray();
            var cchntext = alltext.Where(v => v.LangID == cchidx).ToArray();
            var engtext = alltext.Where(v => v.LangID == engidx).ToArray();
            foreach (var k in allkey) {
                TCHNLang.Add(k.LKey, k.Item);
                SCHNLang.Add(k.LKey, k.Item);
                EngLang.Add(k.LKey, k.Item);
                if (tchntext.Any(v => v.LangTextID == k.ID && v.Text != "")) {
                    TCHNLang[k.LKey] = tchntext.Where(v => v.LangTextID == k.ID).First().Text;
                }
                if (cchntext.Any(v => v.LangTextID == k.ID && v.Text != ""))
                {
                    SCHNLang[k.LKey] = cchntext.Where(v => v.LangTextID == k.ID).First().Text;
                }
                if (engtext.Any(v => v.LangTextID == k.ID && v.Text != ""))
                {
                    EngLang[k.LKey] = engtext.Where(v => v.LangTextID == k.ID).First().Text;
                }
            }
       
            var chn = ldata.Where(v => v.Lang_Name == "繁體中文");
            if (chn.Count() > 0) {
                if (LangDict.ContainsKey(chn.First().ID.ToString()))
                {
                    LangDict[chn.First().ID.ToString()] = TCHNLang;
                }
                else {
                    LangDict.Add(chn.First().ID.ToString(), TCHNLang);
                }
            }
            var eng = ldata.Where(v => v.Lang_Name == "英文");
            if (eng.Count() > 0) {
                if (LangDict.ContainsKey(chn.First().ID.ToString()))
                {
                    LangDict[eng.First().ID.ToString()] = EngLang;
                }
                else
                {
                    LangDict.Add(eng.First().ID.ToString(), EngLang);
                }
            }
            var chs = ldata.Where(v => v.Lang_Name == "簡體中文");
            if (chs.Count() > 0) { 
                if (LangDict.ContainsKey(chn.First().ID.ToString()))
                {
                    LangDict[chs.First().ID.ToString()] = SCHNLang;
                }
                else
                {
                    LangDict.Add(chs.First().ID.ToString(), SCHNLang);
                }
            }
        }
        #endregion

        #region GetLangText
        public static string GetLangText(string key)
        {
            var langid = HttpContext.Current.Session["LangID"];
            Dictionary<string, string> tempdict = null;
            tempdict = TCHNLang;
            if (langid != null  && LangDict != null)
            {
                if (LangDict.ContainsKey(langid.ToString())) {
                    tempdict = LangDict[langid.ToString()];
                }
            }
            if (tempdict == null)
            {
                return key;
            }
            else {
                if (tempdict.ContainsKey(key))
                {
                    if (tempdict[key] != "") { return tempdict[key]; }
                }
            }
            return key;
        }
        #endregion

        #region GetLangDict
        public static Dictionary<string, string> GetLangDict()
        {
            var langid = HttpContext.Current.Session["LangID"];
            Dictionary<string, string> tempdict = null;
            tempdict = TCHNLang;
            if (langid != null && LangDict != null)
            {
                if (LangDict.ContainsKey(langid.ToString()))
                {
                    tempdict = LangDict[langid.ToString()];
                }
            }
            return tempdict;
        }
        #endregion

        #region Replace
        public static string Replace(string expression, string Find , string Replacement)
        {return expression.Replace(Find, Replacement);}
        #endregion

        #region ValidRecaptcha
        public static bool ValidRecaptcha(string token)
        {
            var client = new System.Net.WebClient();
            var googleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", recaptchakeyv2, token));
            dynamic Json = JsonConvert.DeserializeObject(googleReply);
            var success = Json.success;
            return success == "true" ? true : false;
        }
        #endregion

        #region GetMD5
        public static string GetMD5(string sstr)
        {
            string salted = "NCHC";
            byte[] Original = Encoding.Default.GetBytes(sstr);
            byte[] SaltValue = Encoding.Default.GetBytes(salted);
            byte[] ToSalt = new byte[Original.Length + SaltValue.Length];
            Original.CopyTo(ToSalt, 0);
            SaltValue.CopyTo(ToSalt, Original.Length);
            System.Security.Cryptography.MD5 st = System.Security.Cryptography.MD5.Create();
            byte[] SaltPWD = st.ComputeHash(ToSalt);
            byte[] PWD = new byte[SaltPWD.Length + SaltValue.Length];
            SaltPWD.CopyTo(PWD, 0);
            SaltValue.CopyTo(PWD, SaltPWD.Length);
            return Convert.ToBase64String(PWD);
        } 
        #endregion

        #region ReplaceBr
        public static string ReplaceBr(string source)
        {
            return source.ReplaceBr();
        }
        #endregion

        #region FixLink
        public static string FixLink(string source)
        {
            if (source.IsNullorEmpty()) { return "#"; }
            if (source.ToLower().IndexOf("http") >= 0)
            {
                return source;
            }
            else if (source.ToLower().IndexOf("~") == 0)
            {
                UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                return helper.Content(source);
            }
            else if (source.ToLower().IndexOf("/") == 0)
            {
                UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                return helper.Content(source);
            }
            else
            {
                return VirtualPathUtility.ToAbsolute("~/") + source;
            }
        } 
        #endregion
    }
}