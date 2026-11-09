using Microsoft.AspNetCore.SignalR;

namespace MyAcademy_ApiProject.Hubs
{
    // Canlı skor kanalı: /hubs/match
    // Sunucu -> istemci olayları (MatchesController gönderir):
    //   "MatchChanged" { matchId, week, kind }      -> maç eklendi / güncellendi / silindi / toplu skor
    //   "GoalScored"   { matchId, week, side, ... }  -> skor arttı, gol animasyonu oynatılır
    public class MatchHub : Hub
    {
    }
}
