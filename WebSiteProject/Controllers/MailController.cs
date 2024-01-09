using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;

namespace WebSiteProject.Controllers
{
    public class MailController : AppController
    {
        readonly SQLRepository<FormSetting> _formsettingsqlrepository;
        public MailController()
        {
            _formsettingsqlrepository = new SQLRepository<FormSetting>(connectionstr);
        }

        #region SendMail
        public ActionResult SendMail(string Sender, string SenderEMail, string ForwardEMail, string ForwardMessage,string Url,string Title)
        {
            try
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
                string body = Sender + Common.GetLangText("寄了一則訊息給你喔") +"<br/> "+ Common.GetLangText("給您的訊息")  +":" + ForwardMessage +
                    "<br/>" + Url;
                message.Body = body;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;

                SmtpClient client = new SmtpClient(host);
                client.Send(message);
                return Json("寄信完成");

            }
            catch (Exception ex) {
                return Json("寄信失敗:" + ex.Message);
            }
        }
        #endregion
    }
}