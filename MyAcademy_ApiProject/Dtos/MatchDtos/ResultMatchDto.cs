namespace MyAcademy_ApiProject.Dtos.MatchDtos
{
    public class ResultMatchDto
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
    }
}
