using Microsoft.AspNetCore.Mvc;
using MyAcademy_ApiWebUIProject.Services;

namespace MyAcademy_ApiWebUIProject.Controllers
{
    public class StandingsController(IApiService _apiService) : Controller
    {
        // /Standings          -> güncel puan durumu
        // /Standings?week=11  -> 11. hafta sonundaki puan durumu
        public async Task<IActionResult> Index(int? week)
        {
            var summary = await _apiService.GetStandingsSummaryAsync(week);
            if (summary == null)
            {
                return RedirectToAction(nameof(Index));   // o haftada maç yoksa güncel tabloya dön
            }

            summary.History = await _apiService.GetStandingsHistoryAsync();
            return View(summary);
        }
    }
}
