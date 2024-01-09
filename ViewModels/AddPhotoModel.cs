using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class AddPhotoModel
    {
        public AddPhotoModel() {
            ItemID = -1;
        }
        public int ItemID { get; set; }
        public int MainID { get; set; }
        public HttpPostedFileBase[] files { get; set; }
        public string [] text { get; set; }
    }
}
