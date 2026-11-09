namespace MyAcademy_ApiWebUIProject.ViewModels
{
    // API: GET /api/standings/history -> sıralama grafiği (hafta hafta takımların sırası)
    public class StandingsHistoryViewModel
    {
        public List<int> Weeks { get; set; } = new();
        public List<TeamHistoryViewModel> Teams { get; set; } = new();
    }

    public class TeamHistoryViewModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamCode { get; set; } = string.Empty;
        public string ColorA { get; set; } = "#1E5BD8";
        public string ColorB { get; set; } = "#FFFFFF";
        public List<int> Positions { get; set; } = new();
        public List<int> Points { get; set; } = new();
    }
}
