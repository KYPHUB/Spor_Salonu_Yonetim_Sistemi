using FitnessApp.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Web.Services;

public class AppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeSpan>> GetAvailableSlotsAsync(int trainerId, DateTime date, int durationMinutes)
    {
        // Geçmiş tarih kontrolü
        if (date.Date < DateTime.Today)
        {
            return new List<TimeSpan>();
        }

        // Eğitmenin o günkü çalışma saatlerini al
        var availability = await _context.TrainerAvailabilities
            .FirstOrDefaultAsync(a => a.TrainerId == trainerId && a.DayOfWeek == date.DayOfWeek);

        if (availability == null)
        {
            return new List<TimeSpan>();
        }

        // O günkü mevcut randevuları al
        var existingAppointments = await _context.Appointments
            .Where(a => a.TrainerId == trainerId && a.Date.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
            .ToListAsync();

        var slots = new List<TimeSpan>();
        var currentTime = availability.StartTime;
        var endTime = availability.EndTime;

        // Eğer bugünse, şu anki saatten öncesini ele
        if (date.Date == DateTime.Today)
        {
            var now = DateTime.Now.TimeOfDay;
            if (currentTime < now)
            {
                // Bir sonraki tam saat veya yarım saate yuvarla (opsiyonel, şimdilik direkt current'ı ilerletelim)
                // Basitçe current'ı now'dan büyük olana kadar ilerletmek yerine,
                // döngü içinde kontrol edeceğiz.
            }
        }

        while (currentTime.Add(TimeSpan.FromMinutes(durationMinutes)) <= endTime)
        {
            var slotEnd = currentTime.Add(TimeSpan.FromMinutes(durationMinutes));

            // Geçmiş saat kontrolü (Bugün için)
            if (date.Date == DateTime.Today && currentTime < DateTime.Now.TimeOfDay)
            {
                currentTime = currentTime.Add(TimeSpan.FromMinutes(30)); // 30 dk aralıklarla slot oluştur
                continue;
            }

            // Çakışma Kontrolü
            bool isConflict = existingAppointments.Any(a =>
                (currentTime >= a.StartTime && currentTime < a.EndTime) || // Slot başlangıcı mevcut randevu içinde
                (slotEnd > a.StartTime && slotEnd <= a.EndTime) || // Slot bitişi mevcut randevu içinde
                (currentTime <= a.StartTime && slotEnd >= a.EndTime) // Slot mevcut randevuyu kapsıyor
            );

            if (!isConflict)
            {
                slots.Add(currentTime);
            }

            currentTime = currentTime.Add(TimeSpan.FromMinutes(30)); // 30 dakikalık aralıklarla slot öner
        }

        return slots;
    }
}
