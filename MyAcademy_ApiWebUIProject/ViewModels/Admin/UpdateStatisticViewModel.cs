namespace MyAcademy_ApiWebUIProject.ViewModels.Admin
{
    // Admin > Maç İstatistikleri formu: istatistik alanları + başlıkta gösterilecek maç bilgisi
    public class UpdateStatisticViewModel : MatchStatisticViewModel
    {
        public int MatchId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
    }
}
