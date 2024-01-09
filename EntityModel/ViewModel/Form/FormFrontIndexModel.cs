﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ViewModels
{
    public class FormFrontIndexModel : MasterPageModel
    {
        public FormFrontIndexModel() {

        }
        public string ItemID { get; set; }
        public string GroupID { get; set; }
        public string MenuID { get; set; }
        public string Title { get; set; }
        public IList<SelectListItem> GroupList { get; set; }
        public bool Hasgroup { get; set; }
        public string MainID { get; set; }
        public string LinkStr { get; set; }
        public bool IsPrint { get; set; }
        public bool IsForward { get; set; }
        public string MenuType { get; set; }
        public string SiteMenuID { get; set; }
        public string Desc { get; set; }
        public string Formhtml { get; set; }
        public string CatchStr { get; set; }
        
    }
}
