using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.Data;

public class Trainer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad Soyad zorunludur.")]
    public string FullName { get; set; }

    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }

    // Uzmanlık alanları (Hizmetler)
    public List<Service> Specialties { get; set; } = new List<Service>();

    // Çalışma Saatleri
    public List<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();
}
