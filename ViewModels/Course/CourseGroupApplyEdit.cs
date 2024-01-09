using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CourseGroupApplyEdit
    {
        public CourseGroupApplyEdit()
        {
         
        }
        public string ID { get; set; }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public string SNCode { get; set; }
        public string PriceItem { get; set; }
        public string GroupPriceItem { get; set; }
        public bool HasPriceItem { get; set; }
        public int IsInOffer { get; set; }
        public List<string> BCKey { get; set; }
        public List<string> BCColumnName { get; set; }
        public List<string>  BCColumnType { get; set; }
        public List<bool> BCSingalUse { get; set; }
        public List<bool> BCSingalMust { get; set; }
        public Dictionary<string, List<string>> BCTableItem { get; set; }
        public CoursePaymentSetting CoursePaymentSetting { get; set; }

        public string[] CouponItem { get; set; }
        public string[] CouponPrice { get; set; }
        public Dictionary<string, string> BCValue { get; set; }
        public string Price { get; set; }
        public string PaymentType { get; set; }

        public List<string> SIKey { get; set; }
        public List<string> SIColumnName { get; set; }
        public List<string> SIColumnType { get; set; }

        public List<bool> SIGroupUse { get; set; }
        public List<bool> SIGroupMust { get; set; }
        public Dictionary<string, List<string>> SIGroupTableItem { get; set; }
        public List<Dictionary<string, string>> SIGroupValue { get; set; }
        public string[] GroupSeqIndex { get; set; }
        public string[] GroupPrice { get; set; }
        public Dictionary<string, string> Country { get; set; }
        public Dictionary<string, string> IDentity { get; set; }
        public Dictionary<string, string> Event { get; set; }

        public string PayPrice { get; set; }
        public bool IsSettingPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string PriceType { get; set; }
        public string CustomerPrice { get; set; }
        public bool PaymentNotice { get; set; }
        public string Status { get; set; }
        public string CourseDesc { get; set; }
        public string StudentID { get; set; }
        public bool IsShowPriceItem { get; set; }
        public string RPaymentType { get; set; }
        public string VAccount { get; set; }
        public string ExpireDate { get; set; }
        public string ATMAccBank { get; set; }
        public string RtnCode { get; set; }
    }
}
