using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ProductItemResult
    {
        public int ItemID { get; set; }
        public string Inventory { get; set; }
        public string ProductName { get; set; }
        public string ProductModel { get; set; }
        public string Category { get; set; }
        public bool? Launch { get; set; }
        public int Sort { get; set; }
        public int ClickCnt { get; set; }
        public string ImageUrl { get; set; }
        public string StudentPrice { get; set; }
        public string SuggestionPrice { get; set; }
        public string Title { get; set; }
        public int BuyNum { get; set; }
    }
}
