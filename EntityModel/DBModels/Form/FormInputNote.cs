using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class FormInputNote
    {
        public int InputID { get; set; }
        public int MainID { get; set; }
        public string NoteText { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public string Account { get; set; }
        public string Type { get; set; }
    }
}
