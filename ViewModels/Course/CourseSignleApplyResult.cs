using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseSignleApplyResult
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int MainID { get; set; }
        public string CName { get; set; }
        public string SNCode { get; set; }
        public string Title { get; set; }
        public string SInfo { get; set; }
        public string SItem { get; set; }
        public string Status { get; set; }
        public string StatusStrs { get; set; }
        public string PStatus { get; set; }
        public string PaymentStatus { get; set; }
        public bool PaymentNotice { get; set; }
        public bool CourseNotice { get; set; }
        public bool NoteNotice { get; set; }
        public string ActiveDesc { get; set; }
        public string ActiveRange { get; set; }
        public string DownloadCertificate { get; set; }
        public string SigninType { get; set; }
        public string isOverTime { get; set; }
        public string SettingPriceChk { get; set; }
        public string CourseNoticeStr { get; set; }
        public string NoteNoticeStr { get; set; }
        public string PaymentNoticeStr { get; set; }
    }
}
