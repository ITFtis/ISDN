using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class Member
    {
        public int? MemberID { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Code { get; set; }
        public bool? Status { get; set; }
        public bool? Active { get; set; }
        public string CompanyName { get; set; }
        public int? PrepaidFee { get; set; }
        public string ApplyDatetime { get; set; }
        public string IssueDatetime { get; set; }
        public int? IndustryId { get; set; }
        public int? IndustryItemId { get; set; }
        public int? CompanyId { get; set; }
        public string ContactName { get; set; }
        public string ContactId { get; set; }
        public int? CapacityId { get; set; }
        public int? LevelId { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? RetreatDatetime { get; set; }
        public string IndustryItemList { get; set; }
    }
}
