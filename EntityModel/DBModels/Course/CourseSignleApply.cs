using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CourseSignleApply
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int? ItemID { get; set; }
        public int? MainID { get; set; }       
        public string SNCode { get; set; }
        public string Price { get; set; }
        public string PayPrice { get; set; }
        public string CustomerPrice { get; set; }
        public string PaymentType { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string SigninType { get; set; }
        public string CouponItem { get; set; }
        public string CouponPrice { get; set; }
        public int? Status { get; set; }
        public int? PaymentStatus { get; set; }
        public bool? PaymentNotice { get; set; }
        public int? CourseNotice { get; set; }
        public int? NoteNotice { get; set; }
        public DateTime? Updatetime { get; set; }
        public string UpdateUser { get; set; }
        public int? SeqIndex { get; set; }
        public int CouponTotalPrice { get; set; }
        public string WebATMAccBank { get; set; }
        public string WebATMAccNo { get; set; }
        public string WebATMBankName { get; set; }
        public string ATMAccBank { get; set; }
        public string ATMAccNo { get; set; }
        public string PaymentNo { get; set; }
        public string PayFrom { get; set; }
        public string Card4no { get; set; }
        public string Card6no { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public string TradeNo { get; set; }
        public string MerchantTradeNo { get; set; }
        public string PaymentDate { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string TradeDate { get; set; }
        public string TradeAmt { get; set; }
        public string StoreID { get; set; }
        public string PaymentTypeChargeFee { get; set; }
        public string MenuID { get; set; }
        public string StudentID { get; set; }
        public string CourseDesc { get; set; }
        public string TaxID { get; set; }
        public string CourseName { get; set; }
        public string ActiveDateStr { get; set; }
        public string VAccount { get; set; }
        public string ExpireDate { get; set; }
        public string RPaymentType { get; set; }
    }
}


