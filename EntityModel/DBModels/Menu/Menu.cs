using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class Menu
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? ParentID { get; set; }
        public int LangID { get; set; }
        public int ModelID { get; set; }
        public int ModelItemID { get; set; }
        public int MenuLevel { get; set; }
        public int? LinkMode { get; set; }
        public int? ShowMode { get; set; }
        public int? OpenMode { get; set; }
        public int? Sort { get; set; }
        public int? MenuType { get; set; }
        public bool? Status { get; set; }
        public string MenuName { get; set; }
        public int? ImageHeight { get; set; }
        public int? WindowWidth { get; set; }
        public int? WindowHeight { get; set; }
        public string ImgNameOri { get; set; }
        public string ImgShowName { get; set; }
        public string ImgNameThumb { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string MenuUrl { get; set; }
        public string LinkUrl { get; set; }
        public string LinkUploadFileName { get; set; }
        public string LinkUploadFilePath { get; set; }
        public string DisplayName { get; set; }
        public string ICon { get; set; }
    }
}
