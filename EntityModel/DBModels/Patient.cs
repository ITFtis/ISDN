using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class Patient
    {
        [Key]
        public string No { get; set; }
        public string Name { get; set; }
        public string IDNo { get; set; }
        public string BDate { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string BedNo { get; set; }
        public string SNo { get; set; }
        public string HDate { get; set; }
        public string NoticeNo { get; set; }

    }
}
