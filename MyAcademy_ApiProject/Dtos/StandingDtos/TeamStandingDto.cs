namespace MyAcademy_ApiProject.Dtos.StandingDtos
{
    public class TeamStandingDto
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
        public int? PositionChange { get; set; } // önceki haftaya göre: +2 = iki sıra yükseldi, null = önceki hafta yok
        public string Zone { get; set; } = "mid"; // ucl, uel, conf, mid, rel
        public List<string> Form { get; set; } = new(); // ["W", "D", "L", ...]
    }
}
