namespace MyAcademy_ApiProject.Dtos.MatchDtos
{
    public class CardItemDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public int Minute { get; set; }
        public string CardType { get; set; } = "Yellow";
    }
}
