using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;

namespace FitnessApp.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class TrainersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TrainersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Trainers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetTrainers()
    {
        var trainers = await _context.Trainers
            .Include(t => t.Specialties)
            .Select(t => new
            {
                t.Id,
                t.FullName,
                t.Bio,
                Specialties = t.Specialties.Select(s => s.Name).ToList()
            })
            .ToListAsync();

        return Ok(trainers);
    }

    // GET: api/Trainers/filter?date=2025-12-01
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<object>>> FilterTrainers(DateTime? date)
    {
        if (date == null)
        {
            return BadRequest("Date parameter is required.");
        }

        var dayOfWeek = date.Value.DayOfWeek;

        var availableTrainers = await _context.Trainers
            .Include(t => t.Availabilities)
            .Include(t => t.Specialties)
            .Where(t => t.Availabilities.Any(a => a.DayOfWeek == dayOfWeek))
            .Select(t => new
            {
                t.Id,
                t.FullName,
                Specialties = t.Specialties.Select(s => s.Name).ToList(),
                WorkingHours = t.Availabilities
                    .Where(a => a.DayOfWeek == dayOfWeek)
                    .Select(a => new { Start = a.StartTime, End = a.EndTime })
                    .ToList()
            })
            .ToListAsync();

        return Ok(availableTrainers);
    }
}
