namespace MyAcademy_ApiProject.Dtos.MatchDtos
{
    public class CreateMatchDto
    {
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string Status { get; set; } = "NotStarted";
        public string? Stadium { get; set; }
        public string? Referee { get; set; }
        public int? Attendance { get; set; }
        public string? Note { get; set; }
    }
}
