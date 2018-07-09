using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
