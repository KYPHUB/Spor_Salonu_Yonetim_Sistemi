using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;

namespace FitnessApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TrainerAvailabilityController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainerAvailabilityController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/TrainerAvailability/Index/5
    public async Task<IActionResult> Index(int? trainerId)
    {
        if (trainerId == null) return NotFound();

        var trainer = await _context.Trainers
            .Include(t => t.Availabilities)
            .FirstOrDefaultAsync(t => t.Id == trainerId);

        if (trainer == null) return NotFound();

        ViewBag.TrainerId = trainer.Id;
        ViewBag.TrainerName = trainer.FullName;

        return View(trainer.Availabilities.OrderBy(a => a.DayOfWeek).ThenBy(a => a.StartTime).ToList());
    }

    // GET: Admin/TrainerAvailability/Create/5
    public async Task<IActionResult> Create(int? trainerId)
    {
        if (trainerId == null) return NotFound();

        var trainer = await _context.Trainers.FindAsync(trainerId);
        if (trainer == null) return NotFound();

        ViewBag.TrainerId = trainer.Id;
        ViewBag.TrainerName = trainer.FullName;
        
        return View(new TrainerAvailability { TrainerId = trainer.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainerAvailability availability)
    {
        ModelState.Remove("Trainer");
        if (ModelState.IsValid)
        {
            // Çakışma Kontrolü
            bool isOverlap = await _context.TrainerAvailabilities.AnyAsync(a => 
                a.TrainerId == availability.TrainerId && 
                a.DayOfWeek == availability.DayOfWeek &&
                ((availability.StartTime >= a.StartTime && availability.StartTime < a.EndTime) ||
                 (availability.EndTime > a.StartTime && availability.EndTime <= a.EndTime) ||
                 (availability.StartTime <= a.StartTime && availability.EndTime >= a.EndTime))
            );

            if (isOverlap)
            {
                ModelState.AddModelError("", "Bu gün ve saat aralığında zaten bir çalışma saati tanımlı.");
            }
            else
            {
                _context.Add(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { trainerId = availability.TrainerId });
            }
        }
        
        var trainer = await _context.Trainers.FindAsync(availability.TrainerId);
        ViewBag.TrainerId = availability.TrainerId;
        ViewBag.TrainerName = trainer?.FullName;
        
        return View(availability);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var availability = await _context.TrainerAvailabilities.FindAsync(id);
        if (availability != null)
        {
            int trainerId = availability.TrainerId;
            _context.TrainerAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { trainerId = trainerId });
        }
        return NotFound();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDefaultSchedule(int trainerId)
    {
        var trainer = await _context.Trainers.FindAsync(trainerId);
        if (trainer == null) return NotFound();

        // Hafta içi her gün 09:00 - 17:00
        var days = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
        var startTime = new TimeSpan(9, 0, 0);
        var endTime = new TimeSpan(17, 0, 0);

        foreach (var day in days)
        {
            // Çakışma yoksa ekle
            bool exists = await _context.TrainerAvailabilities.AnyAsync(a => 
                a.TrainerId == trainerId && a.DayOfWeek == day);
            
            if (!exists)
            {
                _context.TrainerAvailabilities.Add(new TrainerAvailability
                {
                    TrainerId = trainerId,
                    DayOfWeek = day,
                    StartTime = startTime,
                    EndTime = endTime
                });
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { trainerId = trainerId });
    }
}
