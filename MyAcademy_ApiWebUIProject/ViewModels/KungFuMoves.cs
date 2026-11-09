namespace MyAcademy_ApiWebUIProject.ViewModels
{
    // Her gole kung-fu futbolu tarzında bir hareket adı.
    // Gol Id'sinden hesaplanır: aynı gol her sayfada ve her açılışta aynı hareketi gösterir.
    // Aynı liste wwwroot/js/scenes.js içinde de var (canlı gol sahnesi için) - değişirse ikisini birlikte güncelle.
    public record KungFuMove(string Name, string Icon, string Color, string Description);

    public static class KungFuMoves
    {
        private static readonly KungFuMove[] All =
        {
            new("Ejder Vuruşu",      "🐉", "#FF6A00", "Top ateşten bir ejder gibi ağlara kıvrıldı"),
            new("Kartal Pençesi",    "🦅", "#E8A317", "Havada asılı kalıp yukarıdan pençe gibi indirdi"),
            new("Rüzgar Tekmesi",    "🌪️", "#5BC0EB", "Dönerek vurdu, top rüzgârla birlikte süzüldü"),
            new("Kaplan Sıçrayışı",  "🐯", "#F2994A", "Tek sıçrayışta iki savunmacıyı geçti"),
            new("Kobra Dokunuşu",    "🐍", "#1FC98A", "Son anda, bileğin ucuyla sinsice dokundu"),
            new("Turna Kanadı",      "🕊️", "#C9D4EA", "Uzun, zarif bir vole; top kanat açtı"),
            new("Demir Kafa",        "🛡️", "#A9BCDF", "Kafa vuruşu, kale direğinden sert"),
            new("Gölge Ayak",        "🌑", "#9D7BFF", "Kimse görmeden, gölge gibi sızdı")
        };

        public static readonly KungFuMove Penalty = new("Sessiz Ok", "🎯", "#F2B53B", "Penaltı noktasından tek nefeste");
        public static readonly KungFuMove OwnGoal = new("Ters Akım", "↩️", "#8B9CC0", "Rakibin Chi'si kendi kalesine döndü");

        public static KungFuMove For(int goalId, bool isPenalty, bool isOwnGoal) =>
            isOwnGoal ? OwnGoal
            : isPenalty ? Penalty
            : All[(int)((uint)(goalId * 7 + 3) % All.Length)];
    }
}
