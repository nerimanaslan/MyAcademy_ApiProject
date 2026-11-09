using Microsoft.AspNetCore.Mvc;
using MyAcademy_ApiWebUIProject.Services;
using MyAcademy_ApiWebUIProject.ViewModels;

namespace MyAcademy_ApiWebUIProject.Controllers
{
    public class FixturesController(IApiService _apiService) : Controller
    {

        public async Task<IActionResult> Index(int? week)
        {
            int selectedWeek = week ?? 12;
            var matches = await _apiService.GetMatchesByWeekAsync(selectedWeek);

            // Featured match (e.g. Milan vs Roma in Week 12, or Roma vs Inter in Week 11)
            var featured = matches.FirstOrDefault(m => (m.HomeTeamCode == "MIL" && m.AwayTeamCode == "ROM") 
                                                    || (m.HomeTeamCode == "ROM" && m.AwayTeamCode == "INT") 
                                                    || (m.HomeTeamCode == "INT" && m.AwayTeamCode == "JUV"))
                           ?? matches.FirstOrDefault();

            var viewModel = new FixturesPageViewModel
            {
                SelectedWeek = selectedWeek,
                AvailableWeeks = new List<int> { 11, 12, 13 },
                Matches = matches,
                FeaturedMatch = featured,
                WeekDateRange = selectedWeek switch
                {
                    11 => "31 Ekim - 2 Kasım 2026",
                    12 => "7 Kasım - 9 Kasım 2026",
                    13 => "14 Kasım - 16 Kasım 2026",
                    _ => $"{selectedWeek}. Hafta"
                }
            };

            return View(viewModel);
        }
    }
}
