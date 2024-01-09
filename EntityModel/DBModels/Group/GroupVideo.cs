using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class GroupVideo
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public int? Main_ID { get; set; }
        public int? Site_ID { get; set; }
        public int? Sort { get; set; }
        public string Group_Name { get; set; }
        public bool? Seo_Manage { get; set; }
        public bool? Readonly { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
