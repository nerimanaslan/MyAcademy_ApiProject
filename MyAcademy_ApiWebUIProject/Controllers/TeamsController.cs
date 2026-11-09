using Microsoft.AspNetCore.Mvc;
using MyAcademy_ApiWebUIProject.Services;
using MyAcademy_ApiWebUIProject.ViewModels;

namespace MyAcademy_ApiWebUIProject.Controllers
{
    public class TeamsController(IApiService _apiService) : Controller
    {
        public async Task<IActionResult> Detail(int id)
        {
            var team = await _apiService.GetTeamAsync(id);
            if (team == null)
            {
                return NotFound($"Takım #{id} bulunamadı.");
            }

            var matches = (await _apiService.GetMatchesAsync())
                .Where(m => m.HomeTeamId == id || m.AwayTeamId == id)
                .ToList();
            var standings = await _apiService.GetStandingsAsync();

            // Golcüler: oynanmış maçların detayındaki bu takıma ait goller
            var scorers = new Dictionary<string, int>();
            foreach (var played in matches.Where(m => m.HasScore))
            {
                var detail = await _apiService.GetMatchDetailAsync(played.Id);
                if (detail == null) continue;
                foreach (var goal in detail.Goals.Where(g => g.TeamId == id && !g.IsOwnGoal))
                {
                    scorers[goal.PlayerName] = scorers.GetValueOrDefault(goal.PlayerName) + 1;
                }
            }

            return View(new TeamDetailViewModel
            {
                Team = team,
                Standing = standings.FirstOrDefault(s => s.TeamId == id),
                Matches = matches,
                TopScorers = scorers.OrderByDescending(s => s.Value).ThenBy(s => s.Key)
                                    .Select(s => (s.Key, s.Value)).Take(5).ToList()
            });
        }
    }
}
