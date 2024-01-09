using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ViewModels
{
    public class EventListFrontIndexModel : MasterPageModel
    {
        public EventListFrontIndexModel() {

        }
        public string ItemID { get; set; }
        public string GroupID { get; set; }
        public string MenuID { get; set; }
        public string Title { get; set; }
        public IList<EventListItem> EventListItem { get; set; }
        public bool Hasgroup { get; set; }
        public string MainID { get; set; }
        public string MaxTableCount { get; set; }
        public string LinkStr { get; set; }
        
    }
}
