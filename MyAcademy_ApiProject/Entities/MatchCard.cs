namespace MyAcademy_ApiProject.Entities
{
    public class MatchCard
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match? Match { get; set; }

        public int TeamId { get; set; }
        public Team? Team { get; set; }

        public string PlayerName { get; set; } = string.Empty;
        public int Minute { get; set; }
        public string CardType { get; set; } = "Yellow"; // Yellow, Red
    }
}
