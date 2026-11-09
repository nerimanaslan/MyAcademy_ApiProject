using Microsoft.AspNetCore.Mvc;
using MyAcademy_ApiWebUIProject.Services;
using MyAcademy_ApiWebUIProject.ViewModels;

namespace MyAcademy_ApiWebUIProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController(IApiService _apiService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var teams = await _apiService.GetTeamsAsync();
            var matches = await _apiService.GetMatchesAsync();
            var standings = await _apiService.GetStandingsAsync();

            var finished = matches.Where(m => m.Status == "Finished").ToList();
            var totalGoals = finished.Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0));
            var leader = standings.FirstOrDefault();

            var model = new AdminDashboardViewModel
            {
                TotalTeams = teams.Count,
                ActiveTeams = teams.Count(t => t.Status == "active"),
                TotalMatches = matches.Count,
                FinishedMatches = finished.Count,
                TotalGoals = totalGoals,
                AverageGoalsPerMatch = finished.Count > 0 ? Math.Round((double)totalGoals / finished.Count, 2) : 0,
                LeaderTeam = leader != null ? leader.TeamName : "-",
                LeaderPoints = leader != null ? leader.Points : 0,
                RecentMatches = matches.OrderByDescending(m => m.Week).ThenByDescending(m => m.MatchDate).Take(6).ToList(),
                TopTeams = standings.Take(5).ToList()
            };

            return View(model);
        }
    }
}
