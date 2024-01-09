using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CoursePriceEdit
    {
        public CoursePriceEdit()
        {
         
        }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public bool SettingPriceChk { get; set; }
        public int PriceType { get; set; }
        public bool[] PriceItemChk { get; set; }
        public string[] PriceItemName { get; set; }
        public string[] PriceItemPrice1 { get; set; }
        public string[] PriceItemPrice2 { get; set; }
        public string[] PriceItemPrice3 { get; set; }
        public string[] PriceItemPrice4 { get; set; }
        public int[] SignInType { get; set; }
        public bool DiscountRangeChk { get; set; }
        public bool GroupPriceChk { get; set; }
        public string StDateStr { get; set; }
        public string EdDateStr { get; set; }
        public bool[] GroupChk { get; set; }
        public int[] GroupType { get; set; }
        public string[] GroupFrom { get; set; }
        public string[] GroupTo { get; set; }
        public string[] GroupText { get; set; }
    }

    public class CoursePriceItem
    {
        public CoursePriceItem()
        {

        }
        public int[] ID { get; set; }
        public bool[] Chk { get; set; }
        public string[] Name { get; set; }
        public string[] P1 { get; set; }
        public string[] P2 { get; set; }
        public string[] P3 { get; set; }
        public string[] P4 { get; set; }
    }
}
