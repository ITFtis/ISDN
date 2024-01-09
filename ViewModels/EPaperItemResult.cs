using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EPaperItemResult
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string FormatStr { get; set; }
        public string IsEditStr { get; set; }
        public bool? Enabled { get; set; }
        public int Sort { get; set; }
        public bool? IsEdit { get; set; }
        public string PublicshStr { get; set; }
        public string Introduction { get; set; }
    }
}
