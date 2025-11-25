using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email adresi zorunludur.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
}
