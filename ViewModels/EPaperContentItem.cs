using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class EPaperContentItem
    {
        public EPaperContentItem() {
        }
        public int MenuID { get; set; }
        public int ModelID { get; set; }
        public int ItemID { get; set; }
        public string Title { get; set; }
        public bool Selected { get; set; }
        public bool Enabled { get; set; }
    }
}
