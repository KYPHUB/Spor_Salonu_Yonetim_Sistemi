using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.Data;

public class Service
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    public string Name { get; set; } // Yoga, Pilates, Fitness

    public string Description { get; set; }
    
    [Range(15, 180, ErrorMessage = "Süre 15 ile 180 dakika arasında olmalıdır.")]
    public int DurationMinutes { get; set; } // Dakika cinsinden süre
    
    [Range(0, 10000, ErrorMessage = "Fiyat 0 ile 10000 TL arasında olmalıdır.")]
    public decimal Price { get; set; }
}
