using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class ProductEdit
    {
        public ProductEdit()
        {

        }
        public int MainID { get; set; }
        public int ItemID { get; set; }
        public String StDateStr { get; set; }
        public String EdDateStr { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EdDate { get; set; }
        public String ProductName { get; set; }
        public String ProductModel { get; set; }
        public String SuggestionPrice { get; set; }
        public String StudentPrice { get; set; }
        public String Inventory { get; set; }
        public String ContentTitle { get; set; }
        public String ContentHtml { get; set; }
        public HttpPostedFileBase[] ProductImageFiles { get; set; }
        public String[] ProductImageDesc { get; set; }
        public String[] ProductImageFilePath { get; set; }
        public String[] ProductImageID { get; set; }
        public String[] ProductImageFileName { get; set; }

        public String[] ProductIntroductionTitle{ get; set; }
        public String[] ProductIntroductionDesc { get; set; }
        public String[] ProductIntroductionID { get; set; }
        public String CategoryIDList { get; set; }
        public String CategoryIDListStr { get; set; }

        public String Title { get; set; }
        public String Description { get; set; }
        public String[] Keywords { get; set; }
        public bool Launch { get; set; }
    }
}
