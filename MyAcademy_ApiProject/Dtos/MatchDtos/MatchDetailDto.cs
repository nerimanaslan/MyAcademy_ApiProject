using MyAcademy_ApiProject.Dtos.TeamDtos;

namespace MyAcademy_ApiProject.Dtos.MatchDtos
{
    public class MatchDetailDto
    {
        public int Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public string Status { get; set; } = "Finished";
        public string Stadium { get; set; } = string.Empty;
        public string? Referee { get; set; }
        public int? Attendance { get; set; }
        public string? Note { get; set; }

        public ResultTeamDto HomeTeam { get; set; } = null!;
        public ResultTeamDto AwayTeam { get; set; } = null!;

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public List<GoalItemDto> Goals { get; set; } = new();
        public List<CardItemDto> Cards { get; set; } = new();
        public List<SubstitutionItemDto> Substitutions { get; set; } = new();
        public MatchStatisticDto? Statistic { get; set; }
    }
}
