using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class LogisticsSettingEditModel
    {
        public LogisticsSettingEditModel() {}
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
