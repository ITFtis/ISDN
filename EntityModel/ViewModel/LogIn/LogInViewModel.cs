using ResourceLibrary;
using System.ComponentModel.DataAnnotations;
namespace ViewModels
{
    public class LogInViewModel
    {
        public LogInViewModel() {
            Message = "";
        }
        [Display(Name = "Account", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(10, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resource))]
        public string Account { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(10, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }
        [Display(Name = "驗證碼")]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(4, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resource))]
        public string Number { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }

    }
}
