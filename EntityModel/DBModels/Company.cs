using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class Company
    {
        public int? CompanyId { get; set; }
        public int? MemberId { get; set; }
        public string NameCHN { get; set; }
        public string NameENG { get; set; }
        public string NameCHS { get; set; }
        public string ShortName { get; set; }
        public string TaxId { get; set; }
        public string Establishment { get; set; }
        public string WebSite { get; set; }
        public string Zip { get; set; }
        public string Nationality { get; set; }
        public int? AreaCountyId { get; set; }
        public int? AreaLocationId { get; set; }
        public int? CountyId { get; set; }
        public int? TownId { get; set; }
        public string AddressCHN { get; set; }
        public string AddressENG { get; set; }
        public string AddressCHS { get; set; }
        public string AreaOfFactory { get; set; }
        public string ContactAddress { get; set; }
        public string IntroductionCHN { get; set; }
        public string introductionENG { get; set; }
        public string introductionCHS { get; set; }
        public string Capital { get; set; }
        public string CapitalUnit { get; set; }
        public string CapitalPercentage { get; set; }
        public string CapitalTypeOfOversea { get; set; }
        public string EmployeeNum { get; set; }
        public string LocalWorkerNum { get; set; }
        public string ForeignWorkerNum { get; set; }
        public string DirectWorker { get; set; }
        public string LocalDirectWorker { get; set; }
        public string ForeignDirectWorker { get; set; }
        public string InDirectWorker { get; set; }
        public string LocalInDirectWorker { get; set; }
        public string ForeignInDirectWorker { get; set; }
        public string Certification { get; set; }
        public int? FactoryCountyId { get; set; }
        public int? FactoryTownId { get; set; }
        public string AddressOfFactory { get; set; }
        public string RatioOfCapita { get; set; }
        public int? TypeOfForeignCapital { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
