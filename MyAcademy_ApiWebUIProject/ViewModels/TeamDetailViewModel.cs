namespace MyAcademy_ApiWebUIProject.ViewModels
{
    // Kulüp sayfası (/Teams/Detail/{id}): takım bilgisi + puan tablosundaki yeri + maçları + golcüleri
    public class TeamDetailViewModel
    {
        public TeamViewModel Team { get; set; } = new();
        public TeamStandingViewModel? Standing { get; set; }
        public List<MatchViewModel> Matches { get; set; } = new();
        public List<(string Player, int Goals)> TopScorers { get; set; } = new();

        public List<MatchViewModel> PlayedMatches => Matches.Where(m => m.HasScore).OrderByDescending(m => m.MatchDate).ToList();
        public List<MatchViewModel> UpcomingMatches => Matches.Where(m => !m.HasScore).OrderBy(m => m.MatchDate).ToList();

        public string RegionName => AdminTeamsPageViewModel.Regions.GetValueOrDefault(Team.Region, Team.Region);

        // Takımın oynayacağı derbiler (rakip + derbi adı)
        public List<(MatchViewModel Match, string Name)> Derbies =>
            Matches.Where(m => m.DerbyName != null).Select(m => (m, m.DerbyName!)).ToList();
    }
}
