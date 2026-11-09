namespace MyAcademy_ApiProject.Dtos.StandingDtos
{
    // GET /api/standings/history: her oynanmış hafta sonunda takımların sırası ve puanı
    public class StandingsHistoryDto
    {
        public List<int> Weeks { get; set; } = new();
        public List<TeamHistoryDto> Teams { get; set; } = new();
    }
}
