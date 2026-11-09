namespace MyAcademy_ApiProject.Dtos.StandingDtos
{
    public class TeamHistoryDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamCode { get; set; } = string.Empty;
        public string ColorA { get; set; } = "#1E5BD8";
        public string ColorB { get; set; } = "#FFFFFF";
        public List<int> Positions { get; set; } = new();   // Weeks ile aynı sırada
        public List<int> Points { get; set; } = new();
    }
}
