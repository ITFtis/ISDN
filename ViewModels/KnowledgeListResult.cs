using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class KnowledgeListResult
    {
        public int ID { get; set; }
        public string BookName { get; set; }
        public string  IsRange { get; set; }
        public string GroupName { get; set; }
        public bool? Enabled { get; set; }
        public string ImageFileName { get; set; }
        public string Introduction { get; set; }
        public string Info { get; set; }
        public string UploadFileID { get; set; }
    }
}
