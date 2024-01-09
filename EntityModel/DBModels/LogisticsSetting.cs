using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class LogisticsSetting
    {
        public bool MailingUse { get; set; }
        public bool DeliveryUse { get; set; }
        public bool FreightUse { get; set; }
        public string MailingFee { get; set; }
        public string DeliveryFee { get; set; }
        public string FreightFee { get; set; }
        public string MailingFreeFee { get; set; }
        public string DeliveryFreeFee { get; set; }
        public string FreightFreeFee { get; set; }
    }
}
