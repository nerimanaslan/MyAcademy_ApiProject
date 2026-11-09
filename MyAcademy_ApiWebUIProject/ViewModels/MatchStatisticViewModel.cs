using System.ComponentModel.DataAnnotations;

namespace MyAcademy_ApiWebUIProject.ViewModels
{
    // API: GET /api/matches/{id} -> statistic, PUT /api/matches/{id}/statistics
    public class MatchStatisticViewModel
    {
        [Range(0, 100, ErrorMessage = "0 ile 100 arasında olmalıdır.")] public int HomePossession { get; set; } = 50;
        [Range(0, 100, ErrorMessage = "0 ile 100 arasında olmalıdır.")] public int AwayPossession { get; set; } = 50;
        [Range(0, 60, ErrorMessage = "0 ile 60 arasında olmalıdır.")] public int HomeShots { get; set; }
        [Range(0, 60, ErrorMessage = "0 ile 60 arasında olmalıdır.")] public int AwayShots { get; set; }
        [Range(0, 60, ErrorMessage = "0 ile 60 arasında olmalıdır.")] public int HomeShotsOnTarget { get; set; }
        [Range(0, 60, ErrorMessage = "0 ile 60 arasında olmalıdır.")] public int AwayShotsOnTarget { get; set; }
        [Range(0, 30, ErrorMessage = "0 ile 30 arasında olmalıdır.")] public int HomeCorners { get; set; }
        [Range(0, 30, ErrorMessage = "0 ile 30 arasında olmalıdır.")] public int AwayCorners { get; set; }
        [Range(0, 40, ErrorMessage = "0 ile 40 arasında olmalıdır.")] public int HomeFouls { get; set; }
        [Range(0, 40, ErrorMessage = "0 ile 40 arasında olmalıdır.")] public int AwayFouls { get; set; }
        [Range(0, 20, ErrorMessage = "0 ile 20 arasında olmalıdır.")] public int HomeOffsides { get; set; }
        [Range(0, 20, ErrorMessage = "0 ile 20 arasında olmalıdır.")] public int AwayOffsides { get; set; }

        // İstatistik satırlarını partial'da döngüyle çizmek için (etiket, ev, deplasman)
        public IEnumerable<(string Label, int Home, int Away)> Rows => new[]
        {
            ("Toplam Şut", HomeShots, AwayShots),
            ("İsabetli Şut", HomeShotsOnTarget, AwayShotsOnTarget),
            ("Korner", HomeCorners, AwayCorners),
            ("Faul", HomeFouls, AwayFouls),
            ("Ofsayt", HomeOffsides, AwayOffsides)
        };
    }
}
