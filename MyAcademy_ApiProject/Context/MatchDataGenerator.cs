using MyAcademy_ApiProject.Entities;

namespace MyAcademy_ApiProject.Context
{
    // Örnek veri üretimi: seed ve "rastgele skor ata" aynı kadro listesini ve kuralları kullanır.
    // Random maç Id'si ile beslendiği için aynı maç için her seferinde aynı sonuç üretilir.
    public static class MatchDataGenerator
    {
        // Takım Id -> kadrodan öne çıkan oyuncular (Id'ler ApiContextSeed'deki takımlarla aynı)
        public static readonly Dictionary<int, string[]> Squads = new()
        {
            [1] = new[] { "Mateo Retegui", "Ademola Lookman", "Charles De Ketelaere", "Mario Pašalić", "Éderson", "Davide Zappacosta" },
            [2] = new[] { "Riccardo Orsolini", "Dan Ndoye", "Santiago Castro", "Jens Odgaard", "Lewis Ferguson", "Thijs Dallinga" },
            [3] = new[] { "Zito Luvumbo", "Roberto Piccoli", "Gianluca Gaetano", "Răzvan Marin", "Nicolas Viola" },
            [4] = new[] { "Patrick Cutrone", "Andrea Belotti", "Gabriel Strefezza", "Nico Paz", "Assane Diao" },
            [5] = new[] { "Moise Kean", "Albert Guðmundsson", "Andrea Colpani", "Dodô", "Lucas Beltrán" },
            [6] = new[] { "Giuseppe Ambrosino", "Anthony Partipilo", "Giorgi Kvernadze", "Farès Ghedjemis" },
            [7] = new[] { "Andrea Pinamonti", "Junior Messias", "Ruslan Malinovskyi", "Morten Frendrup", "Vitinha" },
            [8] = new[] { "Lautaro Martínez", "Marcus Thuram", "Nicolò Barella", "Hakan Çalhanoğlu", "Federico Dimarco" },
            [9] = new[] { "Dušan Vlahović", "Kenan Yıldız", "Teun Koopmeiners", "Andrea Cambiaso", "Francisco Conceição" },
            [10] = new[] { "Mattia Zaccagni", "Valentín Castellanos", "Boulaye Dia", "Gustav Isaksen", "Mattéo Guendouzi" },
            [11] = new[] { "Nikola Krstović", "Lameck Banda", "Ante Rebić", "Santiago Pierotti" },
            [12] = new[] { "Rafael Leão", "Christian Pulisic", "Álvaro Morata", "Theo Hernández", "Tijjani Reijnders", "Samuel Chukwueze" },
            [13] = new[] { "Daniel Maldini", "Milan Đurić", "Dany Mota", "Matteo Pessina", "Gianluca Caprari" },
            [14] = new[] { "Romelu Lukaku", "Khvicha Kvaratskhelia", "Scott McTominay", "Giovanni Di Lorenzo", "Giacomo Raspadori" },
            [15] = new[] { "Dennis Man", "Valentin Mihăilă", "Ange-Yoan Bonny", "Adrián Bernabé", "Matteo Cancellieri" },
            [16] = new[] { "Paulo Dybala", "Artem Dovbyk", "Lorenzo Pellegrini", "Matías Soulé", "Stephan El Shaarawy" },
            [17] = new[] { "Domenico Berardi", "Armand Laurienté", "Kristian Thorstvedt", "Samuele Mulattieri" },
            [18] = new[] { "Duván Zapata", "Che Adams", "Antonio Sanabria", "Samuele Ricci", "Nikola Vlašić" },
            [19] = new[] { "Lorenzo Lucca", "Florian Thauvin", "Keinan Davis", "Sandi Lovrić", "Jurgen Ekkelenkamp" },
            [20] = new[] { "Joel Pohjanpalo", "Gaetano Oristanio", "Gianluca Busio", "Christian Gytkjær" }
        };

        private static Random RngFor(int matchId, int salt) => new(matchId * 7919 + salt);

        private static string[] SquadOf(int teamId) =>
            Squads.TryGetValue(teamId, out var squad) ? squad : new[] { "Bilinmeyen Oyuncu" };

        // Skorla birebir uyumlu gol kayıtları (ev ve deplasman golleri ayrı ayrı)
        public static List<MatchGoal> BuildGoals(Match match)
        {
            var rng = RngFor(match.Id, 1);
            var goals = new List<MatchGoal>();
            var usedMinutes = new HashSet<int>();

            void AddGoals(int teamId, int count)
            {
                var squad = SquadOf(teamId);
                for (int i = 0; i < count; i++)
                {
                    int minute;
                    do { minute = rng.Next(3, 91); } while (!usedMinutes.Add(minute));

                    var scorer = squad[rng.Next(squad.Length)];
                    var assist = rng.Next(100) < 65 ? squad[rng.Next(squad.Length)] : null;

                    goals.Add(new MatchGoal
                    {
                        MatchId = match.Id,
                        TeamId = teamId,
                        PlayerName = scorer,
                        Minute = minute,
                        IsPenalty = rng.Next(100) < 10,
                        AssistPlayerName = assist == scorer ? null : assist
                    });
                }
            }

            AddGoals(match.HomeTeamId, match.HomeScore ?? 0);
            AddGoals(match.AwayTeamId, match.AwayScore ?? 0);
            return goals.OrderBy(g => g.Minute).ToList();
        }

        // 1-4 sarı kart, düşük ihtimalle bir kırmızı kart
        public static List<MatchCard> BuildCards(Match match)
        {
            var rng = RngFor(match.Id, 2);
            var cards = new List<MatchCard>();
            int count = rng.Next(1, 5);

            for (int i = 0; i < count; i++)
            {
                var teamId = rng.Next(2) == 0 ? match.HomeTeamId : match.AwayTeamId;
                var squad = SquadOf(teamId);
                cards.Add(new MatchCard
                {
                    MatchId = match.Id,
                    TeamId = teamId,
                    PlayerName = squad[rng.Next(squad.Length)],
                    Minute = rng.Next(10, 90),
                    CardType = "Yellow"
                });
            }

            if (rng.Next(100) < 8)
            {
                var teamId = rng.Next(2) == 0 ? match.HomeTeamId : match.AwayTeamId;
                var squad = SquadOf(teamId);
                cards.Add(new MatchCard { MatchId = match.Id, TeamId = teamId, PlayerName = squad[rng.Next(squad.Length)], Minute = rng.Next(60, 90), CardType = "Red" });
            }

            return cards.OrderBy(c => c.Minute).ToList();
        }

        // Kurallar: isabetli şut >= gol, toplam şut > isabetli şut, topa sahip olma toplamı 100
        public static MatchStatistic BuildStatistic(Match match)
        {
            var rng = RngFor(match.Id, 0);
            int homeGoals = match.HomeScore ?? 0, awayGoals = match.AwayScore ?? 0;

            // Ev sahibi biraz daha fazla topla oynar; farklı kazanan taraf hafif üstün
            int homePossession = Math.Clamp(52 + (homeGoals - awayGoals) * 2 + rng.Next(-8, 9), 32, 68);
            int homeOnTarget = homeGoals + rng.Next(1, 5);
            int awayOnTarget = awayGoals + rng.Next(1, 4);

            return new MatchStatistic
            {
                MatchId = match.Id,
                HomePossession = homePossession,
                AwayPossession = 100 - homePossession,
                HomeShotsOnTarget = homeOnTarget,
                AwayShotsOnTarget = awayOnTarget,
                HomeShots = homeOnTarget + rng.Next(4, 10),
                AwayShots = awayOnTarget + rng.Next(3, 9),
                HomeCorners = rng.Next(2, 10),
                AwayCorners = rng.Next(1, 8),
                HomeFouls = rng.Next(8, 17),
                AwayFouls = rng.Next(9, 18),
                HomeOffsides = rng.Next(0, 5),
                AwayOffsides = rng.Next(0, 5)
            };
        }
    }
}
