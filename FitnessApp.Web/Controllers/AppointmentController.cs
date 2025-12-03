using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;
using FitnessApp.Web.Services;
using FitnessApp.Web.ViewModels;
using System.Security.Claims;

namespace FitnessApp.Web.Controllers;

[Authorize(Roles = "Member")]
public class AppointmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly AppointmentService _appointmentService;

    public AppointmentController(ApplicationDbContext context, AppointmentService appointmentService)
    {
        _context = context;
        _appointmentService = appointmentService;
    }

    // GET: Appointment/Create?trainerId=1&serviceId=2
    public async Task<IActionResult> Create(int? trainerId, int? serviceId)
    {
        if (trainerId == null || serviceId == null)
        {
            // Eğer parametreler yoksa, seçim sayfasına yönlendir (şimdilik ana sayfaya)
            return RedirectToAction("Index", "Home");
        }

        var trainer = await _context.Trainers.FindAsync(trainerId);
        var service = await _context.Services.FindAsync(serviceId);

        if (trainer == null || service == null) return NotFound();

        var model = new AppointmentViewModel
        {
            TrainerId = trainer.Id,
            TrainerName = trainer.FullName,
            ServiceId = service.Id,
            ServiceName = service.Name,
            ServicePrice = service.Price,
            ServiceDuration = service.DurationMinutes,
            Date = DateTime.Today.AddDays(1) // Varsayılan olarak yarını seç
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableSlots(int trainerId, DateTime date, int durationMinutes)
    {
        var slots = await _appointmentService.GetAvailableSlotsAsync(trainerId, date, durationMinutes);
        return Json(slots.Select(s => s.ToString(@"hh\:mm")));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Tekrar çakışma kontrolü (backend validation)
            var slots = await _appointmentService.GetAvailableSlotsAsync(model.TrainerId, model.Date, model.ServiceDuration);
            if (!slots.Contains(model.StartTime))
            {
                ModelState.AddModelError("", "Seçilen saat artık müsait değil. Lütfen başka bir saat seçiniz.");
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var appointment = new Appointment
            {
                TrainerId = model.TrainerId,
                ServiceId = model.ServiceId,
                MemberId = userId,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.StartTime.Add(TimeSpan.FromMinutes(model.ServiceDuration)),
                Status = AppointmentStatus.Pending, // Varsayılan: Bekliyor
                CreatedAt = DateTime.Now
            };

            _context.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation));
        }
        return View(model);
    }

    public IActionResult Confirmation()
    {
        return View();
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var appointments = await _context.Appointments
            .Include(a => a.Trainer)
            .Include(a => a.Service)
            .Where(a => a.MemberId == userId)
            .OrderByDescending(a => a.Date)
            .ThenByDescending(a => a.StartTime)
            .ToListAsync();
        return View(appointments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.MemberId == userId);

        if (appointment != null)
        {
            // Sadece gelecekteki ve iptal edilmemiş randevular iptal edilebilir
            if (appointment.Date >= DateTime.Today && appointment.Status != AppointmentStatus.Cancelled)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToAction(nameof(Index));
    }
}
