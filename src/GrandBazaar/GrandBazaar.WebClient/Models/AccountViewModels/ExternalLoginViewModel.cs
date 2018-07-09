using System.ComponentModel.DataAnnotations;

namespace GrandBazaar.WebClient.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
