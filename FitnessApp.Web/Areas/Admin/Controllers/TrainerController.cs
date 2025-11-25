using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Web.Data;

namespace FitnessApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TrainerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public TrainerController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Trainers.Include(t => t.Specialties).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Services = await _context.Services.ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Trainer trainer, IFormFile? photoFile, int[] selectedServices)
    {
        ModelState.Remove("Specialties");
        if (ModelState.IsValid)
        {
            // Fotoğraf Yükleme
            if (photoFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(photoFile.FileName);
                string extension = Path.GetExtension(photoFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/images/trainers/", fileName);
                
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await photoFile.CopyToAsync(fileStream);
                }
                trainer.PhotoUrl = "/images/trainers/" + fileName;
            }

            // Uzmanlık Alanları (Services) Ekleme
            if (selectedServices != null)
            {
                foreach (var serviceId in selectedServices)
                {
                    var service = await _context.Services.FindAsync(serviceId);
                    if (service != null)
                    {
                        trainer.Specialties.Add(service);
                    }
                }
            }

            _context.Add(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Services = await _context.Services.ToListAsync();
        return View(trainer);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var trainer = await _context.Trainers
            .Include(t => t.Specialties)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (trainer == null) return NotFound();

        ViewBag.Services = await _context.Services.ToListAsync();
        return View(trainer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Trainer trainer, IFormFile? photoFile, int[] selectedServices)
    {
        if (id != trainer.Id) return NotFound();

        ModelState.Remove("Specialties");
        if (ModelState.IsValid)
        {
            try
            {
                var existingTrainer = await _context.Trainers
                    .Include(t => t.Specialties)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (existingTrainer == null) return NotFound();

                // Bilgileri güncelle
                existingTrainer.FullName = trainer.FullName;
                existingTrainer.Bio = trainer.Bio;

                // Fotoğraf güncelleme
                if (photoFile != null)
                {
                    // Eski fotoğrafı sil (opsiyonel, şimdilik kalsın)
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(photoFile.FileName);
                    string extension = Path.GetExtension(photoFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/trainers/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(fileStream);
                    }
                    existingTrainer.PhotoUrl = "/images/trainers/" + fileName;
                }

                // Uzmanlık alanlarını güncelle
                existingTrainer.Specialties.Clear();
                if (selectedServices != null)
                {
                    foreach (var serviceId in selectedServices)
                    {
                        var service = await _context.Services.FindAsync(serviceId);
                        if (service != null)
                        {
                            existingTrainer.Specialties.Add(service);
                        }
                    }
                }

                _context.Update(existingTrainer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainerExists(trainer.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Services = await _context.Services.ToListAsync();
        return View(trainer);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var trainer = await _context.Trainers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (trainer == null) return NotFound();

        return View(trainer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trainer = await _context.Trainers.FindAsync(id);
        if (trainer != null)
        {
            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool TrainerExists(int id)
    {
        return _context.Trainers.Any(e => e.Id == id);
    }
}
