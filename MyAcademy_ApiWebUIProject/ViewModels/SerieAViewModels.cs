namespace MyAcademy_ApiWebUIProject.ViewModels
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string City { get; set; } = string.Empty;
        public string Stadium { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Founded { get; set; }
        public string ColorA { get; set; } = "#1E5BD8";
        public string ColorB { get; set; } = "#FFFFFF";
        public string Region { get; set; } = "north";
        public string Status { get; set; } = "active";
    }

    public class MatchViewModel
    {
        public int Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public string KickoffTime => MatchDate.ToString("HH:mm");
        public string MatchDateFormatted => MatchDate.ToString("yyyy-MM-dd");
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string HomeTeamCode { get; set; } = string.Empty;
        public string HomeColorA { get; set; } = "#1E5BD8";
        public string HomeColorB { get; set; } = "#FFFFFF";

        public int AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string AwayTeamCode { get; set; } = string.Empty;
        public string AwayColorA { get; set; } = "#1E5BD8";
        public string AwayColorB { get; set; } = "#FFFFFF";

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string Status { get; set; } = "NotStarted"; // NotStarted, Live, Finished, Postponed
        public string Stadium { get; set; } = string.Empty;
        public string? Referee { get; set; }
        public int? Attendance { get; set; }
        public string? Note { get; set; }

        public bool HasScore => Status == "Live" || Status == "Finished";
        public string? DerbyName => DerbyInfo.Find(HomeTeamCode, AwayTeamCode);
        public string StatusName => Status switch
        {
            "Finished" => "Tamamlandı",
            "Live" => "Canlı",
            "NotStarted" => "Başlamadı",
            "Postponed" => "Ertelendi",
            _ => Status
        };
    }

    public class MatchDetailViewModel
    {
        public int Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public string Status { get; set; } = "Finished";
        public string Stadium { get; set; } = string.Empty;
        public string? Referee { get; set; }
        public int? Attendance { get; set; }
        public string? Note { get; set; }

        public TeamViewModel HomeTeam { get; set; } = new();
        public TeamViewModel AwayTeam { get; set; } = new();

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public List<GoalViewModel> Goals { get; set; } = new();
        public List<CardViewModel> Cards { get; set; } = new();
        public List<SubstitutionViewModel> Substitutions { get; set; } = new();
        public MatchStatisticViewModel? Statistic { get; set; }

        // Partial'larda tekrar tekrar hesaplanmasın diye takım bazlı listeler
        public List<GoalViewModel> HomeGoals => Goals.Where(g => g.TeamId == HomeTeam.Id).OrderBy(g => g.Minute).ToList();
        public List<GoalViewModel> AwayGoals => Goals.Where(g => g.TeamId == AwayTeam.Id).OrderBy(g => g.Minute).ToList();
        public List<CardViewModel> HomeCards => Cards.Where(c => c.TeamId == HomeTeam.Id).OrderBy(c => c.Minute).ToList();
        public List<CardViewModel> AwayCards => Cards.Where(c => c.TeamId == AwayTeam.Id).OrderBy(c => c.Minute).ToList();
        public List<SubstitutionViewModel> HomeSubs => Substitutions.Where(s => s.TeamId == HomeTeam.Id).OrderBy(s => s.Minute).ToList();
        public List<SubstitutionViewModel> AwaySubs => Substitutions.Where(s => s.TeamId == AwayTeam.Id).OrderBy(s => s.Minute).ToList();

        public int FirstHalfHomeGoals => Goals.Count(g => g.TeamId == HomeTeam.Id && g.Minute <= 45);
        public int FirstHalfAwayGoals => Goals.Count(g => g.TeamId == AwayTeam.Id && g.Minute <= 45);
        public int SecondHalfHomeGoals => (HomeScore ?? 0) - FirstHalfHomeGoals;
        public int SecondHalfAwayGoals => (AwayScore ?? 0) - FirstHalfAwayGoals;

        public string? DerbyName => DerbyInfo.Find(HomeTeam.Code, AwayTeam.Code);

        public bool HomeIsLoser => Status == "Finished" && (HomeScore ?? 0) < (AwayScore ?? 0);
        public bool AwayIsLoser => Status == "Finished" && (AwayScore ?? 0) < (HomeScore ?? 0);

        public string StatusName => Status switch
        {
            "Finished" => "Tamamlandı",
            "Live" => "Canlı",
            "NotStarted" => "Başlamadı",
            "Postponed" => "Ertelendi",
            _ => Status
        };

        // Gol, kart ve değişikliklerin dakikaya göre sıralı tek listesi (maç akışı + zaman çizelgesi)
        public List<MatchEventViewModel> Timeline
        {
            get
            {
                var items = new List<MatchEventViewModel>();

                items.AddRange(Goals.Select(g => new MatchEventViewModel
                {
                    Id = $"ev-goal-{g.Id}",
                    GoalId = g.Id,
                    Move = g.Move,
                    Assist = g.AssistPlayerName,
                    Minute = g.Minute,
                    Type = "goal",
                    IsHome = g.TeamId == HomeTeam.Id,
                    Player = g.PlayerName,
                    Meta = (g.IsPenalty ? "Penaltı golü" : g.IsOwnGoal ? "Kendi kalesine" : "Gol")
                           + (!string.IsNullOrEmpty(g.AssistPlayerName) ? $", asist: {g.AssistPlayerName}" : "")
                }));

                items.AddRange(Cards.Select(c => new MatchEventViewModel
                {
                    Id = $"ev-card-{c.Id}",
                    Minute = c.Minute,
                    Type = "card",
                    IsHome = c.TeamId == HomeTeam.Id,
                    Player = c.PlayerName,
                    Meta = c.CardType == "Red" ? "Kırmızı kart, ihraç" : "Sarı kart",
                    CardType = c.CardType
                }));

                items.AddRange(Substitutions.Select(s => new MatchEventViewModel
                {
                    Id = $"ev-sub-{s.Id}",
                    Minute = s.Minute,
                    Type = "sub",
                    IsHome = s.TeamId == HomeTeam.Id,
                    Player = s.PlayerIn,
                    Meta = s.PlayerOut,
                    PlayerIn = s.PlayerIn,
                    PlayerOut = s.PlayerOut
                }));

                return items.OrderBy(x => x.Minute).ToList();
            }
        }
    }

    public class MatchEventViewModel
    {
        public string Id { get; set; } = string.Empty;
        public int Minute { get; set; }
        public string Type { get; set; } = string.Empty; // goal, card, sub
        public bool IsHome { get; set; }
        public string Player { get; set; } = string.Empty;
        public string Meta { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string PlayerIn { get; set; } = string.Empty;
        public string PlayerOut { get; set; } = string.Empty;

        // Yalnızca goller için (ağır çekim tekrar + kung-fu hareketi)
        public int GoalId { get; set; }
        public KungFuMove? Move { get; set; }
        public string? Assist { get; set; }
    }

    public class GoalViewModel
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public int Minute { get; set; }
        public bool IsPenalty { get; set; }
        public bool IsOwnGoal { get; set; }
        public string? AssistPlayerName { get; set; }

        public KungFuMove Move => KungFuMoves.For(Id, IsPenalty, IsOwnGoal);
    }

    public class CardViewModel
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public int Minute { get; set; }
        public string CardType { get; set; } = "Yellow";
    }

    public class SubstitutionViewModel
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PlayerIn { get; set; } = string.Empty;
        public string PlayerOut { get; set; } = string.Empty;
        public int Minute { get; set; }
    }

    public class TeamStandingViewModel
    {
        public int Position { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamCode { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string ColorA { get; set; } = "#1E5BD8";
        public string ColorB { get; set; } = "#FFFFFF";
        public string Region { get; set; } = "north";
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int Points { get; set; }
        public int? PositionChange { get; set; } // +2 = iki sıra yükseldi
        public string Zone { get; set; } = "mid"; // ucl, uel, conf, mid, rel
        public List<string> Form { get; set; } = new();
    }

    public class StandingsPageViewModel
    {
        public string LeaderTeam { get; set; } = string.Empty;
        public int LeaderPoints { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public int TotalGoals { get; set; }
        public double GoalAveragePerMatch { get; set; }
        public int UclCutoffPoints { get; set; }
        public int RelegationLinePoints { get; set; }
        public int CurrentWeek { get; set; }   // tablonun gösterdiği hafta
        public int LatestWeek { get; set; }    // tamamlanmış maçı olan son hafta
        public List<int> PlayedWeeks { get; set; } = new();
        public List<TeamStandingViewModel> Standings { get; set; } = new();

        // Sıralama grafiği verisi (API: /api/standings/history), controller doldurur
        public StandingsHistoryViewModel History { get; set; } = new();
        public bool IsLatest => CurrentWeek == LatestWeek;
    }

    public class FixturesPageViewModel
    {
        public int SelectedWeek { get; set; }
        public List<int> AvailableWeeks { get; set; } = new() { 11, 12, 13 };
        public List<MatchViewModel> Matches { get; set; } = new();
        public MatchViewModel? FeaturedMatch { get; set; }
        public int TotalMatchesThisWeek => Matches.Count;
        public int FinishedMatchesCount => Matches.Count(m => m.Status == "Finished");
        public int GoalsThisWeek => Matches.Where(m => m.Status == "Finished").Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0));
        public string WeekDateRange { get; set; } = string.Empty;
    }

    public class AdminMatchesPageViewModel
    {
        public List<MatchViewModel> Matches { get; set; } = new();
        public List<TeamViewModel> Teams { get; set; } = new();
        public int TotalMatches => Matches.Count;
        public int FinishedCount => Matches.Count(m => m.Status == "Finished");
        public int LiveCount => Matches.Count(m => m.Status == "Live");
        public int NotStartedCount => Matches.Count(m => m.Status == "NotStarted");
        public int PostponedCount => Matches.Count(m => m.Status == "Postponed");
        public int CurrentWeek => Matches.Any() ? Matches.Max(m => m.Week) : 12;
    }

    public class AdminTeamsPageViewModel
    {
        public List<TeamViewModel> Teams { get; set; } = new();     // filtrelenmiş liste (tablo)
        public List<TeamViewModel> AllTeams { get; set; } = new();  // filtresiz liste (lig özeti şeridi)

        public static readonly Dictionary<string, string> Regions = new()
        {
            ["north"] = "Kuzey İtalya",
            ["center"] = "Orta İtalya",
            ["south"] = "Güney İtalya",
            ["island"] = "Adalar"
        };
        public int TotalTeams => Teams.Count;
        public int ActiveTeams => Teams.Count(t => t.Status == "active");
        public int InactiveTeams => Teams.Count(t => t.Status == "inactive");
    }

    public class AdminDashboardViewModel
    {
        public int TotalTeams { get; set; }
        public int ActiveTeams { get; set; }
        public int TotalMatches { get; set; }
        public int FinishedMatches { get; set; }
        public int TotalGoals { get; set; }
        public double AverageGoalsPerMatch { get; set; }
        public string LeaderTeam { get; set; } = string.Empty;
        public int LeaderPoints { get; set; }
        public List<MatchViewModel> RecentMatches { get; set; } = new();
        public List<TeamStandingViewModel> TopTeams { get; set; } = new();
    }
}
