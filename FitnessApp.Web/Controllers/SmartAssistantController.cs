using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessApp.Web.Services;
using FitnessApp.Web.ViewModels;

namespace FitnessApp.Web.Controllers;

[Authorize(Roles = "Member")]
public class SmartAssistantController : Controller
{
    private readonly IAIService _aiService;

    public SmartAssistantController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new UserStatsViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(UserStatsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var plan = await _aiService.GeneratePlanAsync(model);
        return View("Result", plan); // Pass the plan string directly to the view
    }
}
