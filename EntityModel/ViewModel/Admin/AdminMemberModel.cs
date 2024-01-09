using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
   public  class AdminMemberModel
    {
        public AdminMemberModel()
        {
            ID = -1;
            EncryptID = "";
        }
        public int ID { get; set; }
        public string EncryptID { get; set; }
        [Display(Name = "Account", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resource))]
        public string Account { get; set; }

        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(20, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resource))]
        public string Name { get; set; }
        public int GroupId { get; set; }
        [Display(Name = "Group", ResourceType = typeof(Resource))]
        public string GroupName { get; set; }

        [Display(Name = "EMail", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resource))]
       // [EmailAddress(ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resource))]
        public string Email { get; set; }
        public bool Status { get; set; }
    }
}
