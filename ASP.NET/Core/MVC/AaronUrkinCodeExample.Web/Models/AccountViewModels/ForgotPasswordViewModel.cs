using System.ComponentModel.DataAnnotations;

namespace AaronUrkinCodeExample.Web.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please, enter your Email")]
        [EmailAddress(ErrorMessage = "Please, enter correct Email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
