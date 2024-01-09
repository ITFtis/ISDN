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

    public class LangController : AppController
    {
       ILangManager _ILangManager;
        public LangController()
        {
            _ILangManager = serviceinstance.LangManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            Session["IsFromClick"] = "Y";
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            return View();
        }

        //====Main
        #region EditUnit
        public ActionResult EditUnit(string mainid, string name)
        {
            if (Request.IsAuthenticated)
            {
                var str = "";
                if (mainid == "-1")
                {
                    var newid = 0; Common.SetLogs(this.UserID, this.Account, "新增語系單元名稱=" + name);
                    str = _ILangManager.AddUnit(name, this.LanguageID, this.Account,ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改語系單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _ILangManager.UpdateUnit(name, mainid, this.Account);
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
            return Json(_ILangManager.PagingMain(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更語系切換單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_ILangManager.UpdateMainSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列語系=" + delaccount);
                return Json(_ILangManager.MainDelete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region ModelItem
        [AuthoridUrl("Model/Index", "")]
        public ActionResult ModelItem(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (Session["IsFromClick"] != null)
            {
                ViewBag.IsFromClick = "Y";
            }

            ViewBag.langlist = _ILangManager.GetSelectList();
            ViewBag.mainid = mainid;
            var olddmaindata = _ILangManager.GetModelLangById(mainid);
            ViewBag.langid = olddmaindata.UseLangID==null?-1 : olddmaindata.UseLangID.Value;
            ViewBag.usetype = olddmaindata.UseType == null ? -1 : olddmaindata.UseType.Value;
            return View();
        }
        #endregion

        #region SetModelSetting
        public ActionResult SetModelSetting(string mainid,string type,string langid)
        {
            _ILangManager.ChangeLangType(mainid, type, langid);
            return Json("儲存成功");
        }
        #endregion
        
    }
}