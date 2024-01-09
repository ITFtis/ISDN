using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class EventListUnitSetting
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int MainID { get; set; }
        public bool IsPrint { get; set; }
        public bool IsForward { get; set; }
        public bool IsRSS { get; set; }
        public bool IsShare { get; set; }
        public bool MemberAuth { get; set; }
        public bool ClassOverview { get; set; }
        public int? ShowCount { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public bool GeneralStudentAuth { get; set; }
        public bool VIPAuth { get; set; }
        public bool EMailAuth { get; set; }
        public bool EnterpriceStudentAuth { get; set; }
        public string ColumnSetting { get; set; }
        public string IntroductionHtml { get; set; }
        
    }
}
