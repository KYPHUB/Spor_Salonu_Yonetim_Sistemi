using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.Data;

public class Service
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    public string Name { get; set; } // Yoga, Pilates, Fitness

    public string Description { get; set; }
    
    public int DurationMinutes { get; set; } // Dakika cinsinden süre
    
    public decimal Price { get; set; }
}
