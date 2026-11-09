namespace MyAcademy_ApiWebUIProject.ViewModels
{
    // İtalya'nın bilinen derbileri: iki takım kodu (sırası önemsiz) -> derbi adı
    public static class DerbyInfo
    {
        private static readonly Dictionary<string, string> Derbies = new()
        {
            [Key("MIL", "INT")] = "Derby della Madonnina",
            [Key("ROM", "LAZ")] = "Derby della Capitale",
            [Key("JUV", "TOR")] = "Derby della Mole",
            [Key("INT", "JUV")] = "Derby d'Italia",
            [Key("MIL", "JUV")] = "Klasik: Milan - Juventus",
            [Key("NAP", "ROM")] = "Derby del Sole",
            [Key("FIO", "BOL")] = "Derby dell'Appennino"
        };

        private static string Key(string a, string b) =>
            string.CompareOrdinal(a, b) < 0 ? $"{a}-{b}" : $"{b}-{a}";

        public static string? Find(string homeCode, string awayCode) =>
            Derbies.TryGetValue(Key(homeCode, awayCode), out var name) ? name : null;
    }
}
