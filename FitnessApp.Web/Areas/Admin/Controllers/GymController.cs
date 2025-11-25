using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;

namespace FitnessApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class GymController : Controller
{
    private readonly ApplicationDbContext _context;

    public GymController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Gyms.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Gym gym)
    {
        if (ModelState.IsValid)
        {
            _context.Add(gym);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(gym);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var gym = await _context.Gyms.FindAsync(id);
        if (gym == null) return NotFound();
        return View(gym);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Gym gym)
    {
        if (id != gym.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(gym);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GymExists(gym.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(gym);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var gym = await _context.Gyms
            .FirstOrDefaultAsync(m => m.Id == id);
        if (gym == null) return NotFound();

        return View(gym);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var gym = await _context.Gyms.FindAsync(id);
        if (gym != null)
        {
            _context.Gyms.Remove(gym);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool GymExists(int id)
    {
        return _context.Gyms.Any(e => e.Id == id);
    }
}
