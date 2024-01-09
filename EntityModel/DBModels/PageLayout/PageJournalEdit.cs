using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PageJournalEdit
    {
        [Key]
        public int LangID { get; set; }
        public int SelModelID { get; set; }
        public int SelModelItemID { get; set; }
        public string Title { get; set; }
        public string TitleEng { get; set; }
        public string ModelItemList { get; set; }
    }
}
