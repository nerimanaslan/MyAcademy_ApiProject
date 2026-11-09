namespace MyAcademy_ApiProject.Entities
{
    // Bir maçın takım istatistikleri (maç ile bire bir ilişki)
    public class MatchStatistic
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match? Match { get; set; }

        public int HomePossession { get; set; }   // yüzde, Home + Away = 100
        public int AwayPossession { get; set; }

        public int HomeShots { get; set; }
        public int AwayShots { get; set; }

        public int HomeShotsOnTarget { get; set; }
        public int AwayShotsOnTarget { get; set; }

        public int HomeCorners { get; set; }
        public int AwayCorners { get; set; }

        public int HomeFouls { get; set; }
        public int AwayFouls { get; set; }

        public int HomeOffsides { get; set; }
        public int AwayOffsides { get; set; }
    }
}
