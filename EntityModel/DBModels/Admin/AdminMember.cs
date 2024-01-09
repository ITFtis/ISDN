using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class AdminMember
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int? GroupId { get; set; }
        public string Email { get; set; }
        public string ExtTel { get; set; }
        public string ManagerIdList { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
