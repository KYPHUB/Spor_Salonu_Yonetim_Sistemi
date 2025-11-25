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
            // Basit çakışma kontrolü (opsiyonel ama iyi olur)
            // Şimdilik direkt ekleyelim
            _context.Add(availability);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { trainerId = availability.TrainerId });
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
}
