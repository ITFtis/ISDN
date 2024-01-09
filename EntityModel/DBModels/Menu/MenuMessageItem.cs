using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class MenuMessageItem
    {
        [Key]
        [IsSequence]
        public int ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? Lang_ID { get; set; }
        public string Title { get; set; }
        public string ItemName { get; set; }
        public string HtmlContent { get; set; }
        public int MenuID { get; set; }
    }
}
