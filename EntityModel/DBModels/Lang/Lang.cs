using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class Lang
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? Lang_ID { get; set; }
        public int? Site_ID { get; set; }
        public int? Sort { get; set; }
        public int? Area_id { get; set; }
        public string Lang_Name { get; set; }
        public string Disp_Name { get; set; }
        public string Domain_Type { get; set; }
        public string Sub_Domain_Name { get; set; }
        public string Indep_Domain_Name { get; set; }
        public string Content_Source { get; set; }
        public int? Link_Lang_ID { get; set; }
        public string Link_Href { get; set; }
        public string Img_Name_Ori { get; set; }
        public string Img_Name_Thumb { get; set; }
        public int? Img_Name_Height { get; set; }
        public string Img_Name_Ori2 { get; set; }
        public string Img_Name_Thumb2 { get; set; }
        public int? Img_Name_Height2 { get; set; }
        public string Img_Name_Ori3 { get; set; }
        public string Img_Name_Thumb3 { get; set; }
        public int? Img_Name_Height3 { get; set; }
        public string Img_Name_Ori10 { get; set; }
        public string Img_Name_Thumb10 { get; set; }
        public string Pda_Footer_Text { get; set; }
        public string Index_Text { get; set; }
        public string Footer_Text { get; set; }
        public bool? Acc_Check_Passed { get; set; }
        public bool? Acc_Check_Passed2 { get; set; }
        public bool? Readonly { get; set; }
        public bool? Enabled { get; set; }
        public bool? Published { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
