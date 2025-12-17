using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;
using FitnessApp.Web.Services;
using FitnessApp.Web.ViewModels;
using System.Security.Claims;
using System.Text.Json;

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

    // GET: Appointment/Create?trainerId=1
    public async Task<IActionResult> Create(int? trainerId, int? serviceId)
    {
        if (trainerId == null) return RedirectToAction("Index", "Home");

        // Antrenörü ve uzmanlıklarını çekiyoruz
        var trainer = await _context.Trainers
            .Include(t => t.Specialties)
            .FirstOrDefaultAsync(t => t.Id == trainerId);

        if (trainer == null) return NotFound();

        // Eğer antrenörün hiç uzmanlığı yoksa hata mesajı
        if (trainer.Specialties == null || !trainer.Specialties.Any())
        {
             return Content("HATA: Bu antrenörün tanımlanmış bir uzmanlık alanı bulunmamaktadır.");
        }

        // Varsayılan seçili hizmeti belirle:
        // Eğer URL'den serviceId geldiyse ve hoca bunu veriyorsa onu seç, yoksa ilkini seç.
        var selectedService = trainer.Specialties.FirstOrDefault(s => s.Id == serviceId) 
                              ?? trainer.Specialties.First();

        var model = new AppointmentViewModel
        {
            TrainerId = trainer.Id,
            TrainerName = trainer.FullName,
            ServiceId = selectedService.Id, // Başlangıçta seçili olan
            ServiceName = selectedService.Name,
            ServicePrice = selectedService.Price,
            ServiceDuration = selectedService.DurationMinutes,
            Date = DateTime.Today.AddDays(1)
        };

        // 1. Dropdown için liste (Text: Hizmet Adı, Value: ID)
        ViewBag.ServiceList = new SelectList(trainer.Specialties, "Id", "Name", selectedService.Id);

        // 2. JavaScript'in fiyat ve süreyi bilmesi için tüm detayları JSON olarak gönderiyoruz
        ViewBag.ServiceDetails = JsonSerializer.Serialize(trainer.Specialties.Select(s => new { s.Id, s.Price, s.DurationMinutes }));

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
        // Modelden gelen ServiceId'yi kullanarak fiyat ve süreyi veritabanından tekrar çekmek en güvenlisidir.
        // Ancak basitlik adına hidden inputlardan gelen veriyi veya tekrar sorguyu kullanabiliriz.
        // Burada tutarlılık için tekrar veritabanından çekip kontrol ediyoruz (Güvenlik Önlemi).
        
        var service = await _context.Services.FindAsync(model.ServiceId);
        if (service != null)
        {
            model.ServiceDuration = service.DurationMinutes; // Süreyi güncelle (JS manipülasyonuna karşı)
        }

        if (ModelState.IsValid)
        {
            var slots = await _appointmentService.GetAvailableSlotsAsync(model.TrainerId, model.Date, model.ServiceDuration);
            if (!slots.Contains(model.StartTime))
            {
                ModelState.AddModelError("", "Seçilen saat artık müsait değil. Lütfen başka bir saat seçiniz.");
                
                // Hata durumunda View'ı tekrar doldurmamız lazım (Dropdownlar boş gelmesin)
                var trainer = await _context.Trainers.Include(t => t.Specialties).FirstOrDefaultAsync(t => t.Id == model.TrainerId);
                if (trainer != null)
                {
                    ViewBag.ServiceList = new SelectList(trainer.Specialties, "Id", "Name", model.ServiceId);
                    ViewBag.ServiceDetails = JsonSerializer.Serialize(trainer.Specialties.Select(s => new { s.Id, s.Price, s.DurationMinutes }));
                }
                
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
                Status = AppointmentStatus.Pending,
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
            if (appointment.Date >= DateTime.Today && appointment.Status != AppointmentStatus.Cancelled)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToAction(nameof(Index));
    }
}   