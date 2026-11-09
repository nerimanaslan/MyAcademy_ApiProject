namespace MyAcademy_ApiProject.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // e.g. MIL, INT, JUV
        public string? LogoUrl { get; set; }
        public string City { get; set; } = string.Empty;
        public string Stadium { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Founded { get; set; }
        public string ColorA { get; set; } = "#1E5BD8";
        public string ColorB { get; set; } = "#FFFFFF";
        public string Region { get; set; } = "north"; // north, center, south, island
        public string Status { get; set; } = "active"; // active, inactive
    }
}
