using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;

namespace WebSiteProject.Areas.webadmin.Controllers
{

    public class SEOController : AppController
    {
        readonly SQLRepository<SEO> _sqlrepository;
        public SEOController()
        {
            _sqlrepository  =  new SQLRepository<SEO>(connectionstr);
        }
        [AuthoridUrl("SEO/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var seodata = _sqlrepository.GetByWhere("TypeName=@1 and Lang_ID=@2", new object[] { "Main",this.LanguageID });
            var model = new SEOViewModel();
            if (seodata.Count() > 0)
            {
                model.ID = seodata.First().ID;
                model.Description = seodata.First().Description;
                model.WebsiteTitle = seodata.First().Title;
                model.Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10};
            }
            else {
                model.Keywords = new string[10];
            }
            return View(model);
        }

        public ActionResult Save(SEOViewModel model)
        {
            model.Description = HttpUtility.UrlDecode(model.Description);
            var seomodel = new SEO()
            {
                Description = model.Description==null?"" : model.Description,
                Keywords1 = model.Keywords[0],
                Keywords2 = model.Keywords[1],
                Keywords3 = model.Keywords[2],
                Keywords4 = model.Keywords[3],
                Keywords5 = model.Keywords[4],
                Keywords6 = model.Keywords[5],
                Keywords7 = model.Keywords[6],
                Keywords8 = model.Keywords[7],
                Keywords9 = model.Keywords[8],
                Keywords10 = model.Keywords[9],
                Title = model.WebsiteTitle == null ? "" : model.WebsiteTitle,
                TypeName = "Main",
                Lang_ID = int.Parse(this.LanguageID)
            };
            var r = 0;
            if (model.ID == -1)
            {
                r =_sqlrepository.Create(seomodel);
            }
            else {
                seomodel.ID = model.ID;
                r =_sqlrepository.Update(seomodel);
            }
            if (r > 0)
            {
                return Json("儲存成功");
            }
            else {
                return Json("儲存失敗");
            }
           
        }
    }
}