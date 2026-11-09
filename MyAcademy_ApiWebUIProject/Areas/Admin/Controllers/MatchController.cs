using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAcademy_ApiWebUIProject.Services;
using MyAcademy_ApiWebUIProject.ViewModels;
using MyAcademy_ApiWebUIProject.ViewModels.Admin;

namespace MyAcademy_ApiWebUIProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MatchController(IApiService _apiService) : Controller
    {
        private async Task GetTeamsAsync()
        {
            var teams = await _apiService.GetTeamsAsync(status: "active");

            ViewBag.teams = (from team in teams
                             select new SelectListItem
                             {
                                 Text = team.Name,
                                 Value = team.Id.ToString()
                             }).ToList();
        }

        public async Task<IActionResult> Index(int? week, string? team, string? status)
        {
            var matches = await _apiService.GetMatchesAsync(week, team, status);

            ViewBag.week = week;
            ViewBag.team = team;
            ViewBag.status = status;

            return View(new AdminMatchesPageViewModel { Matches = matches });
        }

        public async Task<IActionResult> Create()
        {
            await GetTeamsAsync();
            return View(new CreateMatchViewModel { MatchDate = DateTime.Today.AddHours(20).AddMinutes(45) });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMatchViewModel model)
        {
            if (model.HomeTeamId != null && model.HomeTeamId == model.AwayTeamId)
            {
                ModelState.AddModelError(nameof(model.AwayTeamId), "Ev sahibi ve deplasman takımı aynı olamaz.");
            }

            if (!ModelState.IsValid)
            {
                await GetTeamsAsync();
                return View(model);
            }

            var (success, error) = await _apiService.CreateMatchAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Maç eklenemedi.");
                await GetTeamsAsync();
                return View(model);
            }

            TempData["Success"] = "Maç başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var match = await _apiService.GetMatchDetailAsync(id);
            if (match == null)
            {
                TempData["Error"] = $"Maç #{id} bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            await GetTeamsAsync();
            return View(new UpdateMatchViewModel
            {
                Id = match.Id,
                Week = match.Week,
                MatchDate = match.MatchDate,
                HomeTeamId = match.HomeTeam.Id,
                AwayTeamId = match.AwayTeam.Id,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                Status = match.Status,
                Stadium = match.Stadium,
                Referee = match.Referee,
                Attendance = match.Attendance,
                Note = match.Note
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateMatchViewModel model)
        {
            if (model.HomeTeamId != null && model.HomeTeamId == model.AwayTeamId)
            {
                ModelState.AddModelError(nameof(model.AwayTeamId), "Ev sahibi ve deplasman takımı aynı olamaz.");
            }

            if (!ModelState.IsValid)
            {
                await GetTeamsAsync();
                return View(model);
            }

            var (success, error) = await _apiService.UpdateMatchAsync(model.Id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Maç güncellenemedi.");
                await GetTeamsAsync();
                return View(model);
            }

            TempData["Success"] = "Maç başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Statistics(int id)
        {
            var match = await _apiService.GetMatchDetailAsync(id);
            if (match == null || (match.Status != "Live" && match.Status != "Finished"))
            {
                TempData["Error"] = "İstatistik yalnızca canlı veya tamamlanan maçlara girilebilir.";
                return RedirectToAction(nameof(Index));
            }

            var s = match.Statistic ?? new MatchStatisticViewModel();
            return View(new UpdateStatisticViewModel
            {
                MatchId = match.Id,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                HomePossession = s.HomePossession,
                AwayPossession = s.AwayPossession,
                HomeShots = s.HomeShots,
                AwayShots = s.AwayShots,
                HomeShotsOnTarget = s.HomeShotsOnTarget,
                AwayShotsOnTarget = s.AwayShotsOnTarget,
                HomeCorners = s.HomeCorners,
                AwayCorners = s.AwayCorners,
                HomeFouls = s.HomeFouls,
                AwayFouls = s.AwayFouls,
                HomeOffsides = s.HomeOffsides,
                AwayOffsides = s.AwayOffsides
            });
        }

        [HttpPost]
        public async Task<IActionResult> Statistics(UpdateStatisticViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _apiService.SaveStatisticAsync(model.MatchId, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "İstatistikler kaydedilemedi.");
                return View(model);
            }

            TempData["Success"] = $"{model.HomeTeamName} - {model.AwayTeamName} istatistikleri kaydedildi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _apiService.DeleteMatchAsync(id);
            if (success)
                TempData["Success"] = $"Maç #{id} silindi.";
            else
                TempData["Error"] = error ?? "Maç silinemedi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RandomizeScores(int? week)
        {
            var (success, message) = await _apiService.RandomizeScoresAsync(week);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index), new { week });
        }
    }
}
