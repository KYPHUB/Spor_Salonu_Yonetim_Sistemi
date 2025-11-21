using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Web.Data;

public class Appointment
{
    public int Id { get; set; }

    public string MemberId { get; set; }
    public AppUser Member { get; set; }

    public int TrainerId { get; set; }
    public Trainer Trainer { get; set; }

    public int ServiceId { get; set; }
    public Service Service { get; set; }

    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
}

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Rejected,
    Completed
}
