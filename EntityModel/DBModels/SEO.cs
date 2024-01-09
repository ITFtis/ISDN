using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class SEO
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public string TypeName { get; set; }
        public int? TypeID { get; set; }
        public int? Lang_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords1 { get; set; }
        public string Keywords2 { get; set; }
        public string Keywords3 { get; set; }
        public string Keywords4 { get; set; }
        public string Keywords5 { get; set; }
        public string Keywords6 { get; set; }
        public string Keywords7 { get; set; }
        public string Keywords8 { get; set; }
        public string Keywords9 { get; set; }
        public string Keywords10 { get; set; }
    }
}
