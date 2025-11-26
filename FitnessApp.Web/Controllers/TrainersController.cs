using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;

namespace FitnessApp.Web.Controllers;

public class TrainersController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var trainers = await _context.Trainers
            .Include(t => t.Specialties)
            .ToListAsync();
        return View(trainers);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var trainer = await _context.Trainers
            .Include(t => t.Specialties)
            .Include(t => t.Availabilities)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (trainer == null) return NotFound();

        return View(trainer);
    }
}
