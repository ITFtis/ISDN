using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class EquipmentInfo
    {
        [Key]
        public int EqpID { get; set; }
        public string EqpName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string EqpNo { get; set; }
        public string PurchaseDate { get; set; }
        public string NoteNo { get; set; }
    }
}
