using ResourceLibrary;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{

    public  class PageNewsEditModel
    {
        public PageNewsEditModel()
        {

        }
        public int SelModelID { get; set; }
        public int SelModelItemID { get; set; }
        public string Title { get; set; }
        public string TitleEng { get; set; }
        public string ModelItemList { get; set; }
        public string MoreInfo { get; set; }
    }

}
