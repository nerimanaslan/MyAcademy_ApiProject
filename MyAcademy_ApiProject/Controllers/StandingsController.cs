using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademy_ApiProject.Context;
using MyAcademy_ApiProject.Dtos.StandingDtos;
using MyAcademy_ApiProject.Entities;

namespace MyAcademy_ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController(ApiContext _context) : ControllerBase
    {
        // GET: /api/standings            -> güncel puan durumu
        // GET: /api/standings?week=11    -> 11. hafta sonundaki puan durumu (zaman makinesi)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamStandingDto>>> GetStandings([FromQuery] int? week)
        {
            var (teams, finished) = await LoadAsync();
            if (week.HasValue && !finished.Any(m => m.Week == week.Value))
                return NotFound(new { message = $"{week}. haftada tamamlanmış maç yok." });

            return Ok(BuildWithMovement(teams, finished, week));
        }

        // GET: /api/standings/summary?week=
        [HttpGet("summary")]
        public async Task<ActionResult<StandingsSummaryDto>> GetStandingsSummary([FromQuery] int? week)
        {
            var (teams, finished) = await LoadAsync();
            var playedWeeks = finished.Select(m => m.Week).Distinct().OrderBy(w => w).ToList();

            if (week.HasValue && !playedWeeks.Contains(week.Value))
                return NotFound(new { message = $"{week}. haftada tamamlanmış maç yok." });

            var selectedWeek = week ?? (playedWeeks.Any() ? playedWeeks.Max() : 0);
            var standings = BuildWithMovement(teams, finished, selectedWeek);
            var included = finished.Where(m => m.Week <= selectedWeek).ToList();

            var totalGoals = included.Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0));
            var leader = standings.FirstOrDefault();

            return Ok(new StandingsSummaryDto
            {
                LeaderTeam = leader?.TeamName ?? "",
                LeaderPoints = leader?.Points ?? 0,
                TotalMatchesPlayed = included.Count,
                TotalGoals = totalGoals,
                GoalAveragePerMatch = included.Count > 0 ? Math.Round((double)totalGoals / included.Count, 2) : 0,
                UclCutoffPoints = standings.Count >= 4 ? standings[3].Points : 0,
                RelegationLinePoints = standings.Count >= 18 ? standings[17].Points : 0,
                CurrentWeek = selectedWeek,
                LatestWeek = playedWeeks.Any() ? playedWeeks.Max() : 0,
                PlayedWeeks = playedWeeks,
                Standings = standings
            });
        }

        // GET: /api/standings/history -> her hafta sonunda her takımın sırası ve puanı (sıralama grafiği için)
        [HttpGet("history")]
        public async Task<ActionResult<StandingsHistoryDto>> GetStandingsHistory()
        {
            var (teams, finished) = await LoadAsync();
            var weeks = finished.Select(m => m.Week).Distinct().OrderBy(w => w).ToList();

            var history = teams.ToDictionary(t => t.Id, t => new TeamHistoryDto
            {
                TeamId = t.Id,
                TeamName = t.Name,
                TeamCode = t.Code,
                ColorA = t.ColorA,
                ColorB = t.ColorB
            });

            foreach (var week in weeks)
            {
                foreach (var row in Calculate(teams, finished.Where(m => m.Week <= week)))
                {
                    history[row.TeamId].Positions.Add(row.Position);
                    history[row.TeamId].Points.Add(row.Points);
                }
            }

            // Son haftadaki sıraya göre (lider önce)
            var ordered = history.Values
                .OrderBy(h => h.Positions.LastOrDefault(int.MaxValue))
                .ToList();

            return Ok(new StandingsHistoryDto { Weeks = weeks, Teams = ordered });
        }

        // ---------- Hesaplama ----------

        private async Task<(List<Team> Teams, List<Match> Finished)> LoadAsync()
        {
            // Puan durumunda yalnızca aktif takımlar
            var teams = await _context.Teams.Where(t => t.Status == "active").AsNoTracking().ToListAsync();

            // İş kuralı: puan durumuna yalnızca "Tamamlandı" maçlar dahil edilir
            var finished = await _context.Matches.Where(m => m.Status == "Finished").AsNoTracking().ToListAsync();

            return (teams, finished);
        }

        // İstenen hafta sonundaki tablo + bir önceki oynanmış haftaya göre sıra değişimi
        private static List<TeamStandingDto> BuildWithMovement(List<Team> teams, List<Match> finished, int? untilWeek)
        {
            var playedWeeks = finished.Select(m => m.Week).Distinct().OrderBy(w => w).ToList();
            var week = untilWeek ?? (playedWeeks.Any() ? playedWeeks.Max() : 0);

            var current = Calculate(teams, finished.Where(m => m.Week <= week));

            var previousWeek = playedWeeks.Where(w => w < week).DefaultIfEmpty(0).Max();
            if (previousWeek > 0)
            {
                var before = Calculate(teams, finished.Where(m => m.Week <= previousWeek))
                    .ToDictionary(s => s.TeamId, s => s.Position);

                foreach (var row in current)
                {
                    if (before.TryGetValue(row.TeamId, out var oldPosition))
                        row.PositionChange = oldPosition - row.Position;   // pozitif = yükseldi
                }
            }

            return current;
        }

        private static List<TeamStandingDto> Calculate(List<Team> teams, IEnumerable<Match> finishedMatches)
        {
            var matches = finishedMatches.OrderBy(m => m.MatchDate).ToList();
            var standings = new List<TeamStandingDto>();

            foreach (var team in teams)
            {
                var item = new TeamStandingDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    TeamCode = team.Code,
                    LogoUrl = team.LogoUrl,
                    ColorA = team.ColorA,
                    ColorB = team.ColorB,
                    Region = team.Region
                };

                var results = new List<string>();
                foreach (var match in matches.Where(m => m.HomeTeamId == team.Id || m.AwayTeamId == team.Id))
                {
                    item.Played++;
                    bool isHome = match.HomeTeamId == team.Id;
                    int scored = isHome ? (match.HomeScore ?? 0) : (match.AwayScore ?? 0);
                    int conceded = isHome ? (match.AwayScore ?? 0) : (match.HomeScore ?? 0);

                    item.GoalsFor += scored;
                    item.GoalsAgainst += conceded;

                    if (scored > conceded) { item.Won++; item.Points += 3; results.Add("W"); }       // Galibiyet 3 puan
                    else if (scored == conceded) { item.Drawn++; item.Points += 1; results.Add("D"); } // Beraberlik 1 puan
                    else { item.Lost++; results.Add("L"); }                                             // Mağlubiyet 0 puan
                }

                item.Form = results.TakeLast(5).ToList();
                standings.Add(item);
            }

            // Sıralama önceliği (case): 1. Puan -> 2. Averaj -> 3. Atılan gol (sonra galibiyet, isim)
            var ordered = standings
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ThenByDescending(s => s.Won)
                .ThenBy(s => s.TeamName)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                var pos = i + 1;
                ordered[i].Position = pos;
                ordered[i].Zone = pos <= 4 ? "ucl"                 // Şampiyonlar Ligi
                                : pos == 5 ? "uel"                 // Avrupa Ligi
                                : pos == 6 ? "conf"                // Konferans Ligi
                                : pos >= ordered.Count - 2 ? "rel" // Son 3: küme düşme
                                : "mid";
            }

            return ordered;
        }
    }
}
