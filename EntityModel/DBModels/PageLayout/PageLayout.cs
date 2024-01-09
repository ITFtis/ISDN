using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class PageLayout
    {
        [Key]
        public int? ID { get; set; }
        public int? LangID { get; set; }
        public int? BlockWidth { get; set; }
        public string Title { get; set; }
        public string ImgNameOri { get; set; }
        public string ImgShowName { get; set; }
        public string ImageUrl { get; set; }
        public string ImageDesc { get; set; }
        public int? Sort { get; set; }
        public bool? Status { get; set; }
        public int? LinkMode { get; set; }
        public int? ModelID { get; set; }
        public int? ModelItemID { get; set; }
        public int? MenuItem { get; set; }
        public int? MenuLevel1 { get; set; }
        public int? MenuLevel2 { get; set; }
        public int? MenuLevel3 { get; set; }
        public int? OpenMode { get; set; }
        public string OpenModeCust { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string SType { get; set; }
        public string MenuUrl { get; set; }
        public string ModelItemList { get; set; }
    }
}
