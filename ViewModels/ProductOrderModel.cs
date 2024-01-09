using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ProductOrderModel
    {
        public ProductOrderModel() {

        }
        public int ID { get; set; }
        public int? StudentID { get; set; }
        public string OrderNo { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public string RtnCode { get; set; }
        public int OrderStatus { get; set; }
        public int PaymentStatus { get; set; }
        public string ModelItemID { get; set; }
        public string MenuID { get; set; }
        public string LogisticsType { get; set; }
        public int ProductCnt { get; set; }
        public int ProductPrice { get; set; }
        public int PayPrice { get; set; }
        public string LogisticsFee { get; set; }
        public string LogisticsFreeFee { get; set; }
        public int CouponTotalPrice { get; set; }
        public int LogisticsPayPrice { get; set; }
        public string CouponList { get; set; }
        public string CouponPriceList { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CityName { get; set; }
        public string Country { get; set; }
        public string InvoiceType { get; set; }
        public string OrderTel { get; set; }
        public string ReceiverName { get; set; }
        public string ZIP { get; set; }
        public string ReceiverTEL { get; set; }
        public string IssueTaxID { get; set; }
        public string TaxID { get; set; }
        public string InvoiceTitle { get; set; }
        public string ProductIDList { get; set; }
        public string ProductBuyNumList { get; set; }
        public string ProductBuyPriceList { get; set; }
        public string StudentName { get; set; }
        public string ProductNameList { get; set; }
        public string PaymentType { get; set; }
        public bool InvoiceSend { get; set; }
        public string InvoiceFilePath { get; set; }
        public string InvoiceFileName { get; set; }
        public int? LogisticsCompanyID { get; set; }
        public string LogisticsNo { get; set; }
        public string OrderDesc { get; set; }
        public string MerchantTradeNo { get; set; }
        public string PaymentDate { get; set; }
        public string Card6no { get; set; }
        public string Card4no { get; set; }
        public string TradeNo { get; set; }
        public string WebATMAccBank { get; set; }
        public string WebATMAccNo { get; set; }
        public string ATMAccNo { get; set; }
        public string ATMAccBank { get; set; }
        public string TradeDate { get; set; }
        public string VAccount { get; set; }
        public string ExpireDate { get; set; }
    }
}
