using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class StudentEdit
    {
        public StudentEdit()
        {
         
        }
        public int ID { get; set; }
        public string Account { get; set; }
        public string Password{ get; set; }
        public string CHNName { get; set; }
        public string ENGName { get; set; }
        public string Gender { get; set; }
        public string EMail { get; set; }
        public bool OrderEPaper { get; set; }
        public string TaxID { get; set; }
        public string CHNCompanyName { get; set; }
        public string ENGCompanyName { get; set; }
        public string ContactTEL { get; set; }
        public string ContactFax { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public bool VIP { get; set; }
        public string VIPSTDate { get; set; }
        public string VIPEDDate { get; set; }
        public string CreateDate { get; set; }
        public string LastLoginTime { get; set; }
        public string Level { get; set; }
        public string CompanyName { get; set; }
    }
}
