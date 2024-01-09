using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseDeledItem
    {
        [Key]
        public int? ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? Lang_ID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public string PublicshDate { get; set; }
        public bool? PublicshStrChk { get; set; }
        public string Code { get; set; }
        public bool? CodeChk { get; set; }
        public string Title { get; set; }
        public bool? TitleChk { get; set; }
        public bool ShowSignUp { get; set; }
        public bool? ShowSignUpChk { get; set; }
        public bool? ActionImageChk { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }
        public string VideoPath { get; set; }
        public bool? VideoPathChk { get; set; }
        [EmptyNull]
        public DateTime? StDate { get; set; }
        [EmptyNull]
        public DateTime? EdDate { get; set; }
        public bool? DisplayDateChk { get; set; }
        public bool? GroupIDChk { get; set; }
        public string Organizer { get; set; }
        public bool? OrganizerChk { get; set; }
        public string CoOrganizer { get; set; }
        public bool? CoOrganizerChk { get; set; }
        public string Participant { get; set; }
        public bool? ParticipantChk { get; set; }
        public string PeopleNumber { get; set; }
        public bool? PeopleNumberChk { get; set; }
        public string Teacher { get; set; }
        public bool? TeacherChk { get; set; }
        public string TeacherIntroduce { get; set; }
        public bool? TeacherIntroduceChk { get; set; }
        public string TeacherImageFileOrgName { get; set; }
        public string TeacherImageFileName { get; set; }
        public bool? ActiveDateChk { get; set; }
        [EmptyNull]
        public DateTime? ActiveStDate { get; set; }
        [EmptyNull]
        public DateTime? ActiveEdDate { get; set; }
        public string ActiveDesc { get; set; }
        public string SingUpEndDate { get; set; }
        public bool? SingUpEndDateChk { get; set; }
        public string ActiveTime { get; set; }
        public bool? ActiveTimeChk { get; set; }
        public string Area { get; set; }
        public bool? AreaChk { get; set; }
        public string Location { get; set; }
        public bool? LocationChk { get; set; }
        public bool? FeeDescChk { get; set; }
        public bool? IntroduceChk { get; set; }
        public bool? NoticeItemChk { get; set; }
        public bool? ContactChk { get; set; }
        public bool? TelChk { get; set; }
        public bool? FaxChk { get; set; }
        public bool? EMailChk { get; set; }
        public bool? GoogleMapChk { get; set; }
        public bool? TrafficeChk { get; set; }
        public bool? NoteChk { get; set; }
        public bool? CertificateChk { get; set; }
        public bool? PaymentChk { get; set; }
        public string FeeDescText { get; set; }
        public string PaymentText { get; set; }
        public string IntroduceText { get; set; }
        public string NoticeItemText { get; set; }
        public string ContactText { get; set; }
        public string EMailText { get; set; }
        public string TrafficeText { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string GoogleMap { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileName { get; set; }
        public string Certificate { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public bool? Enabled { get; set; }
        public bool? TecherImgChk { get; set; }
        public int? SignleCnt { get; set; } //目前已經報名人數
        
    }
}


