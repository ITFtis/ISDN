using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PatentItemResult
    {
        public int ItemID { get; set; }
        public string Title { get; set; }
        public string ClickCount { get; set; }
        public string PublicshDate { get; set; }
        public string  IsRange { get; set; }
        public string GroupName { get; set; }
        public bool? Enabled { get; set; }
        public int Sort { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string RelatceImageFileName { get; set; }
        public string VerifyStr { get; set; }
        public string Year { get; set; }
    }
}
