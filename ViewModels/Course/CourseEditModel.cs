using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class CourseEditModel
    {
        public CourseEditModel() {
            PublicshStr = DateTime.Now.ToString("yyyy/MM/dd");
            PublicshStrChk = true;
            TitleChk = true;
            CertificateChk = true;
            GroupIDChk = true;
            DisplayDateChk = true;
            ShowSignUpChk = true;
            ItemID = -1;
        }
        public int ModelID { get; set; }
        public int ItemID { get; set; }
        public int Group_ID { get; set; }
        public string PublicshStr { get; set; }
        public bool PublicshStrChk { get; set; }
        public string Code { get; set; }
        public bool CodeChk { get; set; }
        public string Title { get; set; }
        public bool TitleChk { get; set; }
        public bool ShowSignUp { get; set; }
        public bool ShowSignUpChk { get; set; }
        public bool ActionImageChk { get; set; }
        public string ImageUrl { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }
        public string VideoPath { get; set; }
        public bool VideoPathChk { get; set; }

        public string StDateStr { get; set; }
        public string EdDateStr { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EdDate { get; set; }
        public bool DisplayDateChk { get; set; }
        public bool GroupIDChk { get; set; }
        public string Organizer { get; set; }
        public bool OrganizerChk { get; set; }
        public string CoOrganizer { get; set; }
        public bool CoOrganizerChk { get; set; }
        public string Participant { get; set; }
        public bool ParticipantChk { get; set; }
        public string PeopleNumber { get; set; }
        public bool PeopleNumberChk { get; set; }
        public string Teacher { get; set; }
        public bool TeacherChk { get; set; }
        public string TeacherIntroduce { get; set; }
        public bool TeacherIntroduceChk { get; set; }
        public bool TeacherImgChk { get; set; }

        public HttpPostedFileBase TeacherImageFile { get; set; }
        public string TeacherImageFileOrgName { get; set; }
        public string TeacherImageFileName { get; set; }
        public string TeacherImageUrl { get; set; }
        
        public bool ActiveDateChk { get; set; }
        public string ActiveStDateStr { get; set; }
        public string ActiveEdDateStr { get; set; }
        public string ActiveDesc { get; set; }
        public bool SingUpEndDateChk { get; set; }
        public string SingUpEndDate { get; set; }
        public bool ActiveTimeChk { get; set; }
        public string ActiveTime { get; set; }
        public string ActiveHours { get; set; }
        public bool AreaChk { get; set; }
        public string Area { get; set; }
        public bool LocationChk { get; set; }
        public string Location { get; set; }
        public bool FeeDescChk { get; set; }
        public bool IntroduceChk { get; set; }
        public bool NoticeItemChk { get; set; }
        public bool ContactChk { get; set; }
        public bool TelChk { get; set; }
        public bool FaxChk { get; set; }
        public bool EMailChk { get; set; }
        public bool GoogleMapChk { get; set; }
        public bool TrafficeChk { get; set; }
        public bool NoteChk { get; set; }
        public bool CertificateChk { get; set; }
        public bool PaymentChk { get; set; }
        public string FeeDescText { get; set; }
        public string IntroduceText { get; set; }
        public string NoticeItemText { get; set; }
        public string ContactText { get; set; }
        public string EMailText { get; set; }
        public string PaymentText { get; set; }
        public string TrafficeText { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string GoogleMap { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileName { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }
        public string Certificate { get; set; }
        public string CertificateDesc { get; set; }
        public bool Enabled { get; set; }
    }
}
