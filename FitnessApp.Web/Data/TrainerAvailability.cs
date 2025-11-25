using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.Data;

public class TrainerAvailability
{
    public int Id { get; set; }

    public int TrainerId { get; set; }
    public Trainer? Trainer { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }
}
