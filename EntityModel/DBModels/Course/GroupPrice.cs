using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class GroupPrice
    {
        public bool GroupChk { get; set; }
        public int GroupType { get; set; }
        public string GroupFrom { get; set; }
        public string GroupTo { get; set; }
        public string GroupText { get; set; }
    }
}


