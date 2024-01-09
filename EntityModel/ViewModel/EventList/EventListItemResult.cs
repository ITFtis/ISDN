using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EventListItemResult
    {
        public int ItemID { get; set; }
        public string Title { get; set; }
        public string ClickCount { get; set; }
        public string PublicshDate { get; set; }
        public string  IsRange { get; set; }
        public string Year { get; set; }
        public bool? Enabled { get; set; }
        public int Sort { get; set; }
        public string LinkUrl { get; set; }
        public string UploadFileName { get; set; }
        public int? Link_Mode { get; set; }
        public string Introduction { get; set; }
        public string RelatceImageFileName { get; set; }
        public bool? Publish { get; set; }
        public string VerifyStr { get; set; }
    }
}
