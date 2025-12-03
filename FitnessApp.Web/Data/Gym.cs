using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.Data;

public class Gym
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Salon adı zorunludur.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Adres zorunludur.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Telefon zorunludur.")]
    public string Phone { get; set; }

    public string OpeningHours { get; set; } // Örn: 09:00 - 22:00
}
