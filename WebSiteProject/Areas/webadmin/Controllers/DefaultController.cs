using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class DefaultController : Controller
    {
        // GET: MyAdmin/Default
        public ActionResult Index()
        {
            return View();
        }
    }
}