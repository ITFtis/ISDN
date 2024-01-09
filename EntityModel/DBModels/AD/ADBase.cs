using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ADBase
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? Lang_ID { get; set; }
        public int? Site_ID { get; set; }
        public int? Sort { get; set; }
        public string Type_ID { get; set; }
        [EmptyNull]
        public DateTime? StDate { get; set; }
        [EmptyNull]
        public DateTime? EdDate { get; set; }
        public string AD_Name { get; set; }
        public string Img_Name_Ori { get; set; }
        public string Img_Name_Thumb { get; set; }
        public int? AD_Width { get; set; }
        public int? AD_Height { get; set; }
        public string Link_Href { get; set; }
        public string Link_Mode { get; set; }
        public bool Fixed { get; set; }
        public DateTime? Review_Date { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string Img_Show_Name { get; set; }
        public string SType { get; set; }

        public string UploadVideoFileName { get; set; }
        public string UploadVideoFilePath { get; set; }
        public string UploadVideoFileDesc { get; set; }
        public string ADDesc { get; set; }
        
    }

    public class ADRightDown : ADBase{}
    public class ADRight : ADBase { }
    public class ADMain : ADBase { }
    public class ADDown : ADBase { }
    public class ADCenter : ADBase { }
    public class ADMobile : ADBase { }
    public class ADMobileBlock : ADBase { }
}
