using System.ComponentModel.DataAnnotations;

namespace MyAcademy_ApiWebUIProject.ViewModels.Admin
{
    public class CreateMatchViewModel
    {
        [Required(ErrorMessage = "Lütfen bir hafta seçin.")]
        [Range(1, 38, ErrorMessage = "Hafta 1 ile 38 arasında olmalıdır.")]
        public int? Week { get; set; }

        [Required(ErrorMessage = "Lütfen maç tarihini ve saatini girin.")]
        public DateTime? MatchDate { get; set; }

        [Required(ErrorMessage = "Lütfen ev sahibi takımı seçin.")]
        public int? HomeTeamId { get; set; }

        [Required(ErrorMessage = "Lütfen deplasman takımını seçin.")]
        public int? AwayTeamId { get; set; }

        [Range(0, 30, ErrorMessage = "Skor 0 veya üzeri olmalıdır.")]
        public int? HomeScore { get; set; }

        [Range(0, 30, ErrorMessage = "Skor 0 veya üzeri olmalıdır.")]
        public int? AwayScore { get; set; }

        public string Status { get; set; } = "NotStarted";
        public string? Stadium { get; set; }
        public string? Referee { get; set; }
        public int? Attendance { get; set; }
        public string? Note { get; set; }
    }
}
