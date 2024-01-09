using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class BaseDataItem
    {
        [Key]
        [IsSequence]
        public int ItemID { get; set; }
        public int? Lang_ID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? ClickCnt { get; set; }
        public string ItemName { get; set; }
        public string UnitDesc { get; set; }
        public bool? Year { get; set; }
        public bool? Quarter { get; set; }
        public bool? Month { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public bool? Enabled { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }
        public string Column8 { get; set; }
        public string Column9 { get; set; }
        public string Column10 { get; set; }
        public string Column11 { get; set; }
        public string Column12 { get; set; }
        public string Column13 { get; set; }
        public string Column14 { get; set; }
        public string Column15 { get; set; }
        public string Column16 { get; set; }
        public string Column17 { get; set; }
        public string Column18 { get; set; }
        public string Column19 { get; set; }
        public string Column20 { get; set; }
        public string Column21 { get; set; }
        public string Column22 { get; set; }
        public string Column23 { get; set; }
        public string Column24 { get; set; }
        public string Column25 { get; set; }
        public string Column26 { get; set; }
        public string Column27 { get; set; }
        public string Column28 { get; set; }
        public string Column29 { get; set; }
        public string Column30 { get; set; }
        public string Column31 { get; set; }
        public string Column32 { get; set; }
        public string Column33 { get; set; }
        public string Column34 { get; set; }
        public string Column35 { get; set; }
        public string Column36 { get; set; }
        public string Column37 { get; set; }
        public string Column38 { get; set; }
        public string Column39 { get; set; }
        public string Column40 { get; set; }
        public string Column41 { get; set; }
        public string Column42 { get; set; }
        public string Column43 { get; set; }
        public string Column44 { get; set; }
        public string Column45 { get; set; }
        public string Column46 { get; set; }
        public string Column47 { get; set; }
        public string Column48 { get; set; }
        public string Column49 { get; set; }
        public string Column50 { get; set; }
        public bool? Use1 { get; set; }
        public bool? Use2 { get; set; }
        public bool? Use3 { get; set; }
        public bool? Use4 { get; set; }
        public bool? Use5 { get; set; }
        public bool? Use6 { get; set; }
        public bool? Use7 { get; set; }
        public bool? Use8 { get; set; }
        public bool? Use9 { get; set; }
        public bool? Use10 { get; set; }
        public bool? Use11 { get; set; }
        public bool? Use12 { get; set; }
        public bool? Use13 { get; set; }
        public bool? Use14 { get; set; }
        public bool? Use15 { get; set; }
        public bool? Use16 { get; set; }
        public bool? Use17 { get; set; }
        public bool? Use18 { get; set; }
        public bool? Use19 { get; set; }
        public bool? Use20 { get; set; }
        public bool? Use21 { get; set; }
        public bool? Use22 { get; set; }
        public bool? Use23 { get; set; }
        public bool? Use24 { get; set; }
        public bool? Use25 { get; set; }
        public bool? Use26 { get; set; }
        public bool? Use27 { get; set; }
        public bool? Use28 { get; set; }
        public bool? Use29 { get; set; }
        public bool? Use30 { get; set; }
        public bool? Use31 { get; set; }
        public bool? Use32 { get; set; }
        public bool? Use33 { get; set; }
        public bool? Use34 { get; set; }
        public bool? Use35 { get; set; }
        public bool? Use36 { get; set; }
        public bool? Use37 { get; set; }
        public bool? Use38 { get; set; }
        public bool? Use39 { get; set; }
        public bool? Use40 { get; set; }
        public bool? Use41 { get; set; }
        public bool? Use42 { get; set; }
        public bool? Use43 { get; set; }
        public bool? Use44 { get; set; }
        public bool? Use45 { get; set; }
        public bool? Use46 { get; set; }
        public bool? Use47 { get; set; }
        public bool? Use48 { get; set; }
        public bool? Use49 { get; set; }
        public bool? Use50 { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
    }
}
