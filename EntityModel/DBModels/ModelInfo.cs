using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ModelInfo
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public string ModelName { get; set; }
        public string ModelKey { get; set; }

    }
}
