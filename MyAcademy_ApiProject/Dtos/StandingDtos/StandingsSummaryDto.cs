namespace MyAcademy_ApiProject.Dtos.StandingDtos
{
    public class StandingsSummaryDto
    {
        public string LeaderTeam { get; set; } = string.Empty;
        public int LeaderPoints { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public int TotalGoals { get; set; }
        public double GoalAveragePerMatch { get; set; }
        public int UclCutoffPoints { get; set; } // 4th place points
        public int RelegationLinePoints { get; set; } // 18th place points
        public int CurrentWeek { get; set; }           // tablonun gösterdiği hafta (seçili hafta)
        public int LatestWeek { get; set; }            // tamamlanmış maçı olan son hafta
        public List<int> PlayedWeeks { get; set; } = new();
        public List<TeamStandingDto> Standings { get; set; } = new();
    }
}
