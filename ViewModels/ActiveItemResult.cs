using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ActiveItemResult
    {
        public int ItemID { get; set; }
        public string Title { get; set; }
        public string ClickCount { get; set; }
        public string PublicshDate { get; set; }
        public string  IsRange { get; set; }
        public string GroupName { get; set; }
        public bool? Enabled { get; set; }
        public int Sort { get; set; }
        public string LinkUrl { get; set; }
        public string UploadFileName { get; set; }
        public int? Link_Mode { get; set; }
        public string ImageFileName { get; set; }
        public string CoverImageFileName { get; set; }
    }
}
