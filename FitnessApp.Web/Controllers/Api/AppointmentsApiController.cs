using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;
using System.Security.Claims;

namespace FitnessApp.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Appointments/my-history
    [HttpGet("my-history")]
    public async Task<ActionResult<IEnumerable<object>>> GetMyHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var appointments = await _context.Appointments
            .Include(a => a.Trainer)
            .Include(a => a.Service)
            .Where(a => a.MemberId == userId)
            .OrderByDescending(a => a.Date)
            .Select(a => new
            {
                a.Id,
                Date = a.Date.ToString("yyyy-MM-dd"),
                Time = $"{a.StartTime:hh\\:mm} - {a.EndTime:hh\\:mm}",
                TrainerName = a.Trainer.FullName,
                ServiceName = a.Service.Name,
                Status = a.Status.ToString()
            })
            .ToListAsync();

        return Ok(appointments);
    }
}
