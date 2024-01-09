using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PageSeminar
    {
        [Key]
        public int LangID { get; set; }
        public DateTime? ActiveDate { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Link { get; set; }
        public int BGStyle { get; set; }
        public string StyleUploadFileName { get; set; }
        public string StyleUploadFilePath { get; set; }
        public string StyleUploadFileNameOrg { get; set; }
    }
}
