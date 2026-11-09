using Microsoft.AspNetCore.Mvc;
using MyAcademy_ApiWebUIProject.Services;

namespace MyAcademy_ApiWebUIProject.Controllers
{
    public class MatchesController(IApiService _apiService) : Controller
    {

        public async Task<IActionResult> Detail(int id)
        {
            var match = await _apiService.GetMatchDetailAsync(id);
            if (match == null)
            {
                return NotFound($"Maç #{id} bulunamadı.");
            }

            return View(match);
        }
    }
}
