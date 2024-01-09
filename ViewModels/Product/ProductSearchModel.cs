using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ProductSearchModel : SearchModelBase
    {
        public ProductSearchModel()
        {
            Sort = "Sort";
        }
        public string CategoryLevel1Id { get; set; }
        public string CategoryLevel2Id { get; set; }
        public string DisplayFrom { get; set; }
        public string DisplayTo { get; set; }
        public string ProductName { get; set; }
        public string ProductModel { get; set; }
        public string Launch { get; set; }
        public string InventoryFrom { get; set; }
        public string InventoryTo { get; set; }
    }
}
