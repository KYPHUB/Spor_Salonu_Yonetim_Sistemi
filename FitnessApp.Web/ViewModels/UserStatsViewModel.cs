using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.ViewModels;

public class UserStatsViewModel
{
    [Required(ErrorMessage = "Yaş alanı zorunludur.")]
    [Range(10, 100, ErrorMessage = "Yaş 10 ile 100 arasında olmalıdır.")]
    public int? Age { get; set; }

    [Required(ErrorMessage = "Kilo alanı zorunludur.")]
    [Range(30, 300, ErrorMessage = "Kilo 30 ile 300 kg arasında olmalıdır.")]
    public double? Weight { get; set; }

    [Required(ErrorMessage = "Boy alanı zorunludur.")]
    [Range(100, 250, ErrorMessage = "Boy 100 ile 250 cm arasında olmalıdır.")]
    public double? Height { get; set; }

    [Required(ErrorMessage = "Cinsiyet seçimi zorunludur.")]
    public string Gender { get; set; } = "Erkek";

    [Required(ErrorMessage = "Hedef seçimi zorunludur.")]
    public string Goal { get; set; } = "Kilo Verme"; // Kilo Verme, Kas Kazanma, Form Koruma

    [Required(ErrorMessage = "Aktivite seviyesi zorunludur.")]
    public string ActivityLevel { get; set; } = "Orta"; // Düşük, Orta, Yüksek
}
