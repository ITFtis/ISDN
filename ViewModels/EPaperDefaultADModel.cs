using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class EPaperDefaultADModel
    {
        public EPaperDefaultADModel() {
            ID = -1;
        }
        public int ID { get; set; }
        public HttpPostedFileBase ADImageFiles { get; set; }
        public string ADName { get; set; }
        public string ADLink { get; set; }
        public string ADID { get; set; }
        public string ADFileName { get; set; }
        public string ADFilePath { get; set; }
        public string ADUrl { get; set; }
    }
}
