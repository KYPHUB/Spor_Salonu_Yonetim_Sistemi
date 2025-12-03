using FitnessApp.Web.ViewModels;

namespace FitnessApp.Web.Services;

public interface IAIService
{
    Task<string> GeneratePlanAsync(UserStatsViewModel stats);
}
