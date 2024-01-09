using ResourceLibrary;
using SQLModel.Attributes;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class FormItemSettingModel
    {
        public FormItemSettingModel() {
             ID = -1;
        }
        public int? ID { get; set; }
        public int? MainID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ItemMode { get; set; }
        public string ItemModeName { get; set; }
        public string ColumnNum { get; set; }
        public string RowNum { get; set; }
        public string TextLength { get; set; }
        public string DefaultText { get; set; }
        public string SelList { get; set; }
    }
}
