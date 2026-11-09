using Microsoft.AspNetCore.Mvc;
using MyAcademy_ApiWebUIProject.Services;
using MyAcademy_ApiWebUIProject.ViewModels;
using MyAcademy_ApiWebUIProject.ViewModels.Admin;

namespace MyAcademy_ApiWebUIProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController(IApiService _apiService) : Controller
    {
        public async Task<IActionResult> Index(string? q, string? region, string? status)
        {
            var allTeams = await _apiService.GetTeamsAsync();
            var teams = allTeams
                .Where(t => string.IsNullOrEmpty(region) || t.Region == region)
                .Where(t => string.IsNullOrEmpty(status) || t.Status == status)
                .ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                teams = teams.Where(t => t.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                                      || t.City.Contains(term, StringComparison.OrdinalIgnoreCase)
                                      || t.Stadium.Contains(term, StringComparison.OrdinalIgnoreCase)
                                      || t.Code.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.q = q;
            ViewBag.region = region;
            ViewBag.status = status;

            return View(new AdminTeamsPageViewModel
            {
                Teams = teams.OrderBy(t => t.Id).ToList(),
                AllTeams = allTeams
            });
        }

        public IActionResult Create()
        {
            return View(new CreateTeamViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _apiService.CreateTeamAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Takım eklenemedi.");
                return View(model);
            }

            TempData["Success"] = $"{model.Name} başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var team = await _apiService.GetTeamAsync(id);
            if (team == null)
            {
                TempData["Error"] = $"Takım #{id} bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(new UpdateTeamViewModel
            {
                Id = team.Id,
                Name = team.Name,
                Code = team.Code,
                LogoUrl = team.LogoUrl,
                City = team.City,
                Stadium = team.Stadium,
                Capacity = team.Capacity,
                Founded = team.Founded,
                ColorA = team.ColorA,
                ColorB = team.ColorB,
                Region = team.Region,
                Status = team.Status
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateTeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _apiService.UpdateTeamAsync(model.Id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Takım güncellenemedi.");
                return View(model);
            }

            TempData["Success"] = $"{model.Name} başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _apiService.DeleteTeamAsync(id);
            if (success)
                TempData["Success"] = "Takım silindi. (Maç geçmişi olan takımlar silinmez, pasife alınır.)";
            else
                TempData["Error"] = error ?? "Takım silinemedi.";

            return RedirectToAction(nameof(Index));
        }
    }
}
