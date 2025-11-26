using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.ViewModels;

public class AppointmentViewModel
{
    [Required]
    public int TrainerId { get; set; }
    public string? TrainerName { get; set; }

    [Required]
    public int ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public decimal ServicePrice { get; set; }
    public int ServiceDuration { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}
