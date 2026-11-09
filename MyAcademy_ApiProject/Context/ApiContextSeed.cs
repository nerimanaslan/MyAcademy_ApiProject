using Microsoft.EntityFrameworkCore;
using MyAcademy_ApiProject.Entities;

namespace MyAcademy_ApiProject.Context
{
    public static class ApiContextSeed
    {
        public static async Task SeedAsync(ApiContext context)
        {
            // Bekleyen migration'ları uygular (veritabanı yoksa oluşturur)
            await context.Database.MigrateAsync();

            if (!await context.Teams.AnyAsync())
            {
                await SeedInitialDataAsync(context);
            }

            // Her açılışta: oynanmış ama detayı eksik maçları tamamlar (mevcut kayıtlara dokunmaz)
            await CompleteMatchDataAsync(context);
        }

        private static async Task SeedInitialDataAsync(ApiContext context)
        {
            // 1. Teams (20 active Serie A teams + 2 inactive)
            var teams = new List<Team>
            {
                new Team { Id = 1,  Name = "Atalanta",   Code = "ATA", City = "Bergamo",   Region = "north",  Stadium = "Gewiss Stadium",                Capacity = 24950, Founded = 1907, ColorA = "#1E6FB8", ColorB = "#111111", Status = "active" },
                new Team { Id = 2,  Name = "Bologna",    Code = "BOL", City = "Bologna",   Region = "north",  Stadium = "Stadio Renato Dall'Ara",        Capacity = 36462, Founded = 1909, ColorA = "#1A2F5A", ColorB = "#A21C26", Status = "active" },
                new Team { Id = 3,  Name = "Cagliari",   Code = "CAG", City = "Cagliari",  Region = "island", Stadium = "Unipol Domus",                  Capacity = 16416, Founded = 1920, ColorA = "#A5162B", ColorB = "#0E2A56", Status = "active" },
                new Team { Id = 4,  Name = "Como",       Code = "COM", City = "Como",      Region = "north",  Stadium = "Stadio Giuseppe Sinigaglia",    Capacity = 13602, Founded = 1907, ColorA = "#0B3E91", ColorB = "#FFFFFF", Status = "active" },
                new Team { Id = 5,  Name = "Fiorentina", Code = "FIO", City = "Firenze",   Region = "center", Stadium = "Stadio Artemio Franchi",        Capacity = 43147, Founded = 1926, ColorA = "#5B2A86", ColorB = "#FFFFFF", Status = "active" },
                new Team { Id = 6,  Name = "Frosinone",  Code = "FRO", City = "Frosinone", Region = "center", Stadium = "Stadio Benito Stirpe",          Capacity = 16227, Founded = 1928, ColorA = "#1C4E9A", ColorB = "#FFD200", Status = "active" },
                new Team { Id = 7,  Name = "Genoa",      Code = "GEN", City = "Genova",    Region = "north",  Stadium = "Stadio Luigi Ferraris",         Capacity = 36599, Founded = 1893, ColorA = "#A2172E", ColorB = "#002B5C", Status = "active" },
                new Team { Id = 8,  Name = "Inter",      Code = "INT", City = "Milano",    Region = "north",  Stadium = "San Siro",                      Capacity = 75817, Founded = 1908, ColorA = "#0A4FA8", ColorB = "#101418", Status = "active" },
                new Team { Id = 9,  Name = "Juventus",   Code = "JUV", City = "Torino",    Region = "north",  Stadium = "Allianz Stadium",               Capacity = 41507, Founded = 1897, ColorA = "#16181D", ColorB = "#E8E8E8", Status = "active" },
                new Team { Id = 10, Name = "Lazio",      Code = "LAZ", City = "Roma",      Region = "center", Stadium = "Stadio Olimpico",               Capacity = 70634, Founded = 1900, ColorA = "#8FD3F4", ColorB = "#FFFFFF", Status = "active" },
                new Team { Id = 11, Name = "Lecce",      Code = "LEC", City = "Lecce",     Region = "south",  Stadium = "Stadio Via del Mare",           Capacity = 31533, Founded = 1908, ColorA = "#F6D000", ColorB = "#D21F26", Status = "active" },
                new Team { Id = 12, Name = "Milan",      Code = "MIL", City = "Milano",    Region = "north",  Stadium = "San Siro",                      Capacity = 75817, Founded = 1899, ColorA = "#C8141E", ColorB = "#111111", Status = "active" },
                new Team { Id = 13, Name = "Monza",      Code = "MON", City = "Monza",     Region = "north",  Stadium = "U-Power Stadium",               Capacity = 16917, Founded = 1912, ColorA = "#D2122E", ColorB = "#FFFFFF", Status = "active" },
                new Team { Id = 14, Name = "Napoli",     Code = "NAP", City = "Napoli",    Region = "south",  Stadium = "Stadio Diego Armando Maradona", Capacity = 54726, Founded = 1926, ColorA = "#1E9BD7", ColorB = "#0C3C78", Status = "active" },
                new Team { Id = 15, Name = "Parma",      Code = "PAR", City = "Parma",     Region = "north",  Stadium = "Stadio Ennio Tardini",          Capacity = 22352, Founded = 1913, ColorA = "#FFD200", ColorB = "#1D3E8A", Status = "active" },
                new Team { Id = 16, Name = "Roma",       Code = "ROM", City = "Roma",      Region = "center", Stadium = "Stadio Olimpico",               Capacity = 70634, Founded = 1927, ColorA = "#8E1F2F", ColorB = "#F0B323", Status = "active" },
                new Team { Id = 17, Name = "Sassuolo",   Code = "SAS", City = "Sassuolo",  Region = "north",  Stadium = "Mapei Stadium",                 Capacity = 21584, Founded = 1920, ColorA = "#00A651", ColorB = "#111111", Status = "active" },
                new Team { Id = 18, Name = "Torino",     Code = "TOR", City = "Torino",    Region = "north",  Stadium = "Stadio Olimpico Grande Torino", Capacity = 27958, Founded = 1906, ColorA = "#7E1B1B", ColorB = "#E9D9B0", Status = "active" },
                new Team { Id = 19, Name = "Udinese",    Code = "UDI", City = "Udine",     Region = "north",  Stadium = "Bluenergy Stadium",             Capacity = 25144, Founded = 1896, ColorA = "#26282C", ColorB = "#FFFFFF", Status = "active" },
                new Team { Id = 20, Name = "Venezia",    Code = "VEN", City = "Venezia",   Region = "north",  Stadium = "Stadio Pier Luigi Penzo",       Capacity = 11150, Founded = 1907, ColorA = "#141414", ColorB = "#F06A1A", Status = "active" },
                new Team { Id = 21, Name = "Sampdoria",  Code = "SAM", City = "Genova",    Region = "north",  Stadium = "Stadio Luigi Ferraris",         Capacity = 36599, Founded = 1946, ColorA = "#1B4FA0", ColorB = "#FFFFFF", Status = "inactive" },
                new Team { Id = 22, Name = "Salernitana",Code = "SAL", City = "Salerno",   Region = "south",  Stadium = "Stadio Arechi",                 Capacity = 37180, Founded = 1919, ColorA = "#7A1E2E", ColorB = "#FFFFFF", Status = "inactive" }
            };

            await context.Database.OpenConnectionAsync();
            try
            {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Teams ON");
                await context.Teams.AddRangeAsync(teams);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Teams OFF");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }

            // 2. Matches (30 matches: 10 per week for Week 11, 12, 13)
            var matches = new List<Match>
            {
                // Week 11 (All finished)
                new Match { Id = 1, Week = 11, HomeTeamId = 16, AwayTeamId = 8,  MatchDate = new DateTime(2026, 10, 31, 18, 0, 0), HomeScore = 1, AwayScore = 2, Status = "Finished", Stadium = "Stadio Olimpico", Referee = "Daniele Doveri", Attendance = 63200 },
                new Match { Id = 2, Week = 11, HomeTeamId = 10, AwayTeamId = 18, MatchDate = new DateTime(2026, 10, 31, 15, 0, 0), HomeScore = 1, AwayScore = 1, Status = "Finished", Stadium = "Stadio Olimpico", Referee = "Marco Di Bello", Attendance = 41500 },
                new Match { Id = 3, Week = 11, HomeTeamId = 14, AwayTeamId = 1,  MatchDate = new DateTime(2026, 10, 31, 20, 45, 0), HomeScore = 2, AwayScore = 0, Status = "Finished", Stadium = "Stadio Diego Armando Maradona", Referee = "Maurizio Mariani", Attendance = 51200 },
                new Match { Id = 4, Week = 11, HomeTeamId = 5,  AwayTeamId = 9,  MatchDate = new DateTime(2026, 11, 1, 12, 30, 0), HomeScore = 0, AwayScore = 2, Status = "Finished", Stadium = "Stadio Artemio Franchi", Referee = "Fabio Maresca", Attendance = 38900 },
                new Match { Id = 5, Week = 11, HomeTeamId = 2,  AwayTeamId = 12, MatchDate = new DateTime(2026, 11, 1, 15, 0, 0), HomeScore = 1, AwayScore = 3, Status = "Finished", Stadium = "Stadio Renato Dall'Ara", Referee = "Luca Pairetto", Attendance = 29500 },
                new Match { Id = 6, Week = 11, HomeTeamId = 7,  AwayTeamId = 4,  MatchDate = new DateTime(2026, 11, 1, 15, 0, 0), HomeScore = 0, AwayScore = 1, Status = "Finished", Stadium = "Stadio Luigi Ferraris", Referee = "Antonio Rapuano", Attendance = 32100 },
                new Match { Id = 7, Week = 11, HomeTeamId = 15, AwayTeamId = 19, MatchDate = new DateTime(2026, 11, 1, 18, 0, 0), HomeScore = 2, AwayScore = 2, Status = "Finished", Stadium = "Stadio Ennio Tardini", Referee = "Davide Massa", Attendance = 18700 },
                new Match { Id = 8, Week = 11, HomeTeamId = 3,  AwayTeamId = 11, MatchDate = new DateTime(2026, 11, 1, 20, 45, 0), HomeScore = 1, AwayScore = 0, Status = "Finished", Stadium = "Unipol Domus", Referee = "Matteo Marchetti", Attendance = 15400 },
                new Match { Id = 9, Week = 11, HomeTeamId = 13, AwayTeamId = 6,  MatchDate = new DateTime(2026, 11, 2, 18, 30, 0), HomeScore = 1, AwayScore = 1, Status = "Finished", Stadium = "U-Power Stadium", Referee = "Gianluca Manganiello", Attendance = 12800 },
                new Match { Id = 10, Week = 11, HomeTeamId = 20, AwayTeamId = 17, MatchDate = new DateTime(2026, 11, 2, 20, 45, 0), HomeScore = 1, AwayScore = 2, Status = "Finished", Stadium = "Stadio Pier Luigi Penzo", Referee = "Alberto Santoro", Attendance = 9800 },

                // Week 12
                new Match { Id = 11, Week = 12, HomeTeamId = 1,  AwayTeamId = 5,  MatchDate = new DateTime(2026, 11, 7, 15, 0, 0), HomeScore = 3, AwayScore = 0, Status = "Finished", Stadium = "Gewiss Stadium", Referee = "Daniele Chiffi", Attendance = 23800 },
                new Match { Id = 12, Week = 12, HomeTeamId = 9,  AwayTeamId = 14, MatchDate = new DateTime(2026, 11, 7, 18, 0, 0), HomeScore = 1, AwayScore = 1, Status = "Finished", Stadium = "Allianz Stadium", Referee = "Simone Sozza", Attendance = 40900 },
                new Match { Id = 13, Week = 12, HomeTeamId = 12, AwayTeamId = 16, MatchDate = new DateTime(2026, 11, 7, 20, 45, 0), HomeScore = 2, AwayScore = 1, Status = "Finished", Stadium = "San Siro", Referee = "Davide Massa", Attendance = 72600 },
                new Match { Id = 14, Week = 12, HomeTeamId = 8,  AwayTeamId = 10, MatchDate = new DateTime(2026, 11, 8, 12, 30, 0), HomeScore = 2, AwayScore = 1, Status = "Finished", Stadium = "San Siro", Referee = "Marco Guida", Attendance = 71400 },
                new Match { Id = 15, Week = 12, HomeTeamId = 18, AwayTeamId = 2,  MatchDate = new DateTime(2026, 11, 8, 15, 0, 0), HomeScore = 2, AwayScore = 1, Status = "Finished", Stadium = "Stadio Olimpico Grande Torino", Referee = "Federico La Penna", Attendance = 22100 },
                new Match { Id = 16, Week = 12, HomeTeamId = 4,  AwayTeamId = 15, MatchDate = new DateTime(2026, 11, 8, 15, 0, 0), HomeScore = 2, AwayScore = 1, Status = "Finished", Stadium = "Stadio Giuseppe Sinigaglia", Referee = "Livio Marinelli", Attendance = 11900 },
                new Match { Id = 17, Week = 12, HomeTeamId = 19, AwayTeamId = 7,  MatchDate = new DateTime(2026, 11, 8, 20, 45, 0), HomeScore = 1, AwayScore = 0, Status = "Finished", Stadium = "Bluenergy Stadium", Referee = "Francesco Fourneau", Attendance = 24100 },
                new Match { Id = 18, Week = 12, HomeTeamId = 17, AwayTeamId = 3,  MatchDate = new DateTime(2026, 11, 9, 18, 30, 0), HomeScore = 2, AwayScore = 2, Status = "Finished", Stadium = "Mapei Stadium", Referee = "Andrea Colombo", Attendance = 18300 },
                new Match { Id = 19, Week = 12, HomeTeamId = 11, AwayTeamId = 13, MatchDate = new DateTime(2026, 11, 9, 20, 45, 0), HomeScore = 0, AwayScore = 1, Status = "Finished", Stadium = "Stadio Via del Mare", Referee = "Rosario Abisso", Attendance = 27800 },
                new Match { Id = 20, Week = 12, HomeTeamId = 6,  AwayTeamId = 20, MatchDate = new DateTime(2026, 11, 9, 20, 45, 0), HomeScore = 1, AwayScore = 1, Status = "Finished", Stadium = "Stadio Benito Stirpe", Referee = "Juan Luca Sacchi", Attendance = 14200 },

                // Week 13 (henüz oynanmadı - güncel hafta)
                new Match { Id = 21, Week = 13, HomeTeamId = 8,  AwayTeamId = 9,  MatchDate = new DateTime(2026, 11, 14, 20, 45, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "San Siro", Referee = "Daniele Doveri" },
                new Match { Id = 22, Week = 13, HomeTeamId = 14, AwayTeamId = 12, MatchDate = new DateTime(2026, 11, 14, 18, 0, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "Stadio Diego Armando Maradona", Referee = "Marco Guida" },
                new Match { Id = 23, Week = 13, HomeTeamId = 16, AwayTeamId = 10, MatchDate = new DateTime(2026, 11, 15, 20, 45, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "Stadio Olimpico", Referee = "Simone Sozza" },
                new Match { Id = 24, Week = 13, HomeTeamId = 5,  AwayTeamId = 1,  MatchDate = new DateTime(2026, 11, 15, 15, 0, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "Stadio Artemio Franchi", Referee = "Davide Massa" },
                new Match { Id = 25, Week = 13, HomeTeamId = 2,  AwayTeamId = 4,  MatchDate = new DateTime(2026, 11, 15, 15, 0, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "Stadio Renato Dall'Ara", Referee = "Luca Pairetto" },
                new Match { Id = 26, Week = 13, HomeTeamId = 18, AwayTeamId = 19, MatchDate = new DateTime(2026, 11, 15, 12, 30, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "Stadio Olimpico Grande Torino", Referee = "Matteo Marchetti" },
                new Match { Id = 27, Week = 13, HomeTeamId = 7,  AwayTeamId = 15, MatchDate = new DateTime(2026, 11, 15, 18, 0, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "Stadio Luigi Ferraris", Referee = "Antonio Rapuano" },
                new Match { Id = 28, Week = 13, HomeTeamId = 3,  AwayTeamId = 6,  MatchDate = new DateTime(2026, 11, 16, 18, 30, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "Unipol Domus", Referee = "Alberto Santoro" },
                new Match { Id = 29, Week = 13, HomeTeamId = 13, AwayTeamId = 17, MatchDate = new DateTime(2026, 11, 16, 20, 45, 0), HomeScore = null, AwayScore = null, Status = "NotStarted", Stadium = "U-Power Stadium", Referee = "Gianluca Manganiello" },
                new Match { Id = 30, Week = 13, HomeTeamId = 20, AwayTeamId = 11, MatchDate = new DateTime(2026, 11, 16, 20, 45, 0), HomeScore = null, AwayScore = null, Status = "Postponed", Stadium = "Stadio Pier Luigi Penzo", Referee = "Livio Marinelli", Note = "Yoğun yağış nedeniyle ertelendi, yeni tarih bekleniyor" }
            };

            await context.Database.OpenConnectionAsync();
            try
            {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Matches ON");
                await context.Matches.AddRangeAsync(matches);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Matches OFF");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }

            // 3. Match Details for Featured Matches:
            // Match 13: Milan 2 - 1 Roma (Week 12)
            var goals = new List<MatchGoal>
            {
                new MatchGoal { MatchId = 13, TeamId = 12, PlayerName = "Rafael Leão", Minute = 27, AssistPlayerName = "Theo Hernández" },
                new MatchGoal { MatchId = 13, TeamId = 16, PlayerName = "Paulo Dybala", Minute = 58, IsPenalty = true },
                new MatchGoal { MatchId = 13, TeamId = 12, PlayerName = "Christian Pulisic", Minute = 74, AssistPlayerName = "Youssouf Fofana" },

                // Match 1: Roma 1 - 2 Inter (Week 11)
                new MatchGoal { MatchId = 1, TeamId = 8,  PlayerName = "Lautaro Martínez", Minute = 18, AssistPlayerName = "Marcus Thuram" },
                new MatchGoal { MatchId = 1, TeamId = 16, PlayerName = "Artem Dovbyk", Minute = 41, AssistPlayerName = "Lorenzo Pellegrini" },
                new MatchGoal { MatchId = 1, TeamId = 8,  PlayerName = "Nicolò Barella", Minute = 65, AssistPlayerName = "Federico Dimarco" },

                // Match 11: Atalanta 3 - 0 Fiorentina (Week 12)
                new MatchGoal { MatchId = 11, TeamId = 1, PlayerName = "Mateo Retegui", Minute = 21, AssistPlayerName = "Ademola Lookman" },
                new MatchGoal { MatchId = 11, TeamId = 1, PlayerName = "Ademola Lookman", Minute = 45, AssistPlayerName = "Éderson" },
                new MatchGoal { MatchId = 11, TeamId = 1, PlayerName = "Charles De Ketelaere", Minute = 69, AssistPlayerName = "Davide Zappacosta" },

                // Match 3: Napoli 2 - 0 Atalanta (Week 11)
                new MatchGoal { MatchId = 3, TeamId = 14, PlayerName = "Khvicha Kvaratskhelia", Minute = 38, AssistPlayerName = "Romelu Lukaku" },
                new MatchGoal { MatchId = 3, TeamId = 14, PlayerName = "Romelu Lukaku", Minute = 77, AssistPlayerName = "Stanislav Lobotka" },

                // Match 5: Bologna 1 - 3 Milan (Week 11)
                new MatchGoal { MatchId = 5, TeamId = 12, PlayerName = "Álvaro Morata", Minute = 12, AssistPlayerName = "Rafael Leão" },
                new MatchGoal { MatchId = 5, TeamId = 2,  PlayerName = "Riccardo Orsolini", Minute = 33, AssistPlayerName = "Dan Ndoye" },
                new MatchGoal { MatchId = 5, TeamId = 12, PlayerName = "Tijjani Reijnders", Minute = 56, AssistPlayerName = "Christian Pulisic" },
                new MatchGoal { MatchId = 5, TeamId = 12, PlayerName = "Samuel Chukwueze", Minute = 88, AssistPlayerName = "Yunus Musah" }
            };
            await context.MatchGoals.AddRangeAsync(goals);

            var cards = new List<MatchCard>
            {
                // Match 13: Milan vs Roma
                new MatchCard { MatchId = 13, TeamId = 16, PlayerName = "Bryan Cristante", Minute = 34, CardType = "Yellow" },
                new MatchCard { MatchId = 13, TeamId = 12, PlayerName = "Fikayo Tomori", Minute = 62, CardType = "Yellow" },
                new MatchCard { MatchId = 13, TeamId = 16, PlayerName = "Gianluca Mancini", Minute = 81, CardType = "Yellow" },
                new MatchCard { MatchId = 13, TeamId = 12, PlayerName = "Theo Hernández", Minute = 89, CardType = "Yellow" },

                // Match 1: Roma vs Inter
                new MatchCard { MatchId = 1, TeamId = 8,  PlayerName = "Henrikh Mkhitaryan", Minute = 29, CardType = "Yellow" },
                new MatchCard { MatchId = 1, TeamId = 16, PlayerName = "Gianluca Mancini", Minute = 55, CardType = "Yellow" },
                new MatchCard { MatchId = 1, TeamId = 8,  PlayerName = "Hakan Çalhanoğlu", Minute = 84, CardType = "Yellow" },

                // Match 11: Atalanta vs Fiorentina
                new MatchCard { MatchId = 11, TeamId = 5, PlayerName = "Lucas Martínez Quarta", Minute = 33, CardType = "Yellow" },
                new MatchCard { MatchId = 11, TeamId = 1, PlayerName = "Éderson", Minute = 51, CardType = "Yellow" },
                new MatchCard { MatchId = 11, TeamId = 5, PlayerName = "Dodô", Minute = 77, CardType = "Yellow" },
                new MatchCard { MatchId = 11, TeamId = 5, PlayerName = "Rolando Mandragora", Minute = 85, CardType = "Red" }
            };
            await context.MatchCards.AddRangeAsync(cards);

            var substitutions = new List<MatchSubstitution>
            {
                // Match 13: Milan vs Roma
                new MatchSubstitution { MatchId = 13, TeamId = 12, PlayerIn = "Álvaro Morata", PlayerOut = "Tammy Abraham", Minute = 68 },
                new MatchSubstitution { MatchId = 13, TeamId = 16, PlayerIn = "Stephan El Shaarawy", PlayerOut = "Lorenzo Pellegrini", Minute = 71 },
                new MatchSubstitution { MatchId = 13, TeamId = 12, PlayerIn = "Yunus Musah", PlayerOut = "Ruben Loftus-Cheek", Minute = 80 },
                new MatchSubstitution { MatchId = 13, TeamId = 16, PlayerIn = "Tommaso Baldanzi", PlayerOut = "Matías Soulé", Minute = 83 },

                // Match 1: Roma vs Inter
                new MatchSubstitution { MatchId = 1, TeamId = 8,  PlayerIn = "Davide Frattesi", PlayerOut = "Henrikh Mkhitaryan", Minute = 60 },
                new MatchSubstitution { MatchId = 1, TeamId = 16, PlayerIn = "Matías Soulé", PlayerOut = "Paulo Dybala", Minute = 72 },
                new MatchSubstitution { MatchId = 1, TeamId = 8,  PlayerIn = "Mehdi Taremi", PlayerOut = "Marcus Thuram", Minute = 78 },

                // Match 11: Atalanta vs Fiorentina
                new MatchSubstitution { MatchId = 11, TeamId = 1, PlayerIn = "Mario Pašalić", PlayerOut = "Lazar Samardžić", Minute = 64 },
                new MatchSubstitution { MatchId = 11, TeamId = 5, PlayerIn = "Moise Kean", PlayerOut = "Andrea Colpani", Minute = 70 },
                new MatchSubstitution { MatchId = 11, TeamId = 1, PlayerIn = "Marco Brescianini", PlayerOut = "Mateo Retegui", Minute = 80 }
            };
            await context.MatchSubstitutions.AddRangeAsync(substitutions);

            await context.SaveChangesAsync();
        }

        // Oynanmış (Canlı/Tamamlandı) maçlarda eksik olanları üretir:
        // - hiç gol kaydı yoksa skorla birebir uyumlu goller
        // - hiç kart kaydı yoksa birkaç kart
        // - istatistik yoksa skorla tutarlı istatistik
        // Elle girilmiş kayıtları değiştirmez; tekrar çalışınca bir şey eklemez.
        private static async Task CompleteMatchDataAsync(ApiContext context)
        {
            var playedMatches = await context.Matches
                .Include(m => m.Goals)
                .Include(m => m.Cards)
                .Include(m => m.Statistic)
                .Where(m => m.Status == "Live" || m.Status == "Finished")
                .ToListAsync();

            foreach (var match in playedMatches)
            {
                if (!match.Goals.Any() && (match.HomeScore ?? 0) + (match.AwayScore ?? 0) > 0)
                    context.MatchGoals.AddRange(MatchDataGenerator.BuildGoals(match));

                if (!match.Cards.Any() && match.Status == "Finished")
                    context.MatchCards.AddRange(MatchDataGenerator.BuildCards(match));

                if (match.Statistic == null)
                    context.MatchStatistics.Add(MatchDataGenerator.BuildStatistic(match));
            }

            await context.SaveChangesAsync();
        }
    }
}
