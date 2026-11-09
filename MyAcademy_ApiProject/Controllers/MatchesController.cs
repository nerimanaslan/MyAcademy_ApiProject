using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyAcademy_ApiProject.Context;
using MyAcademy_ApiProject.Dtos.MatchDtos;
using MyAcademy_ApiProject.Dtos.TeamDtos;
using MyAcademy_ApiProject.Entities;
using MyAcademy_ApiProject.Hubs;

namespace MyAcademy_ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController(ApiContext _context, IHubContext<MatchHub> _hub) : ControllerBase
    {

        // GET: /api/matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultMatchDto>>> GetMatches(
            [FromQuery] int? week,
            [FromQuery] string? team,
            [FromQuery] string? status)
        {
            var query = _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .AsNoTracking()
                .AsQueryable();

            if (week.HasValue)
            {
                query = query.Where(m => m.Week == week.Value);
            }

            if (!string.IsNullOrWhiteSpace(team))
            {
                var lowerTeam = team.Trim().ToLower();
                query = query.Where(m => m.HomeTeam!.Name.ToLower().Contains(lowerTeam) || m.AwayTeam!.Name.ToLower().Contains(lowerTeam));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(m => m.Status.ToLower() == status.Trim().ToLower());
            }

            var list = await query
                .OrderBy(m => m.Week)
                .ThenBy(m => m.MatchDate)
                .Select(m => new ResultMatchDto
                {
                    Id = m.Id,
                    Week = m.Week,
                    MatchDate = m.MatchDate,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam != null ? m.HomeTeam.Name : "",
                    HomeTeamCode = m.HomeTeam != null ? m.HomeTeam.Code : "",
                    HomeColorA = m.HomeTeam != null ? m.HomeTeam.ColorA : "#1E5BD8",
                    HomeColorB = m.HomeTeam != null ? m.HomeTeam.ColorB : "#FFFFFF",
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam != null ? m.AwayTeam.Name : "",
                    AwayTeamCode = m.AwayTeam != null ? m.AwayTeam.Code : "",
                    AwayColorA = m.AwayTeam != null ? m.AwayTeam.ColorA : "#1E5BD8",
                    AwayColorB = m.AwayTeam != null ? m.AwayTeam.ColorB : "#FFFFFF",
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status,
                    Stadium = m.Stadium,
                    Referee = m.Referee,
                    Attendance = m.Attendance,
                    Note = m.Note
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: /api/matches/week/{week}
        [HttpGet("week/{week}")]
        public async Task<ActionResult<IEnumerable<ResultMatchDto>>> GetMatchesByWeek(int week)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .AsNoTracking()
                .Where(m => m.Week == week)
                .OrderBy(m => m.MatchDate)
                .Select(m => new ResultMatchDto
                {
                    Id = m.Id,
                    Week = m.Week,
                    MatchDate = m.MatchDate,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam != null ? m.HomeTeam.Name : "",
                    HomeTeamCode = m.HomeTeam != null ? m.HomeTeam.Code : "",
                    HomeColorA = m.HomeTeam != null ? m.HomeTeam.ColorA : "#1E5BD8",
                    HomeColorB = m.HomeTeam != null ? m.HomeTeam.ColorB : "#FFFFFF",
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam != null ? m.AwayTeam.Name : "",
                    AwayTeamCode = m.AwayTeam != null ? m.AwayTeam.Code : "",
                    AwayColorA = m.AwayTeam != null ? m.AwayTeam.ColorA : "#1E5BD8",
                    AwayColorB = m.AwayTeam != null ? m.AwayTeam.ColorB : "#FFFFFF",
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status,
                    Stadium = m.Stadium,
                    Referee = m.Referee,
                    Attendance = m.Attendance,
                    Note = m.Note
                })
                .ToListAsync();

            return Ok(matches);
        }

        // GET: /api/matches/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDetailDto>> GetMatchDetail(int id)
        {
            var m = await _context.Matches
                .Include(x => x.HomeTeam)
                .Include(x => x.AwayTeam)
                .Include(x => x.Goals).ThenInclude(g => g.Team)
                .Include(x => x.Cards).ThenInclude(c => c.Team)
                .Include(x => x.Substitutions).ThenInclude(s => s.Team)
                .Include(x => x.Statistic)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (m == null)
            {
                return NotFound(new { message = $"Maç #{id} bulunamadı." });
            }

            var detail = new MatchDetailDto
            {
                Id = m.Id,
                Week = m.Week,
                MatchDate = m.MatchDate,
                Status = m.Status,
                Stadium = m.Stadium,
                Referee = m.Referee,
                Attendance = m.Attendance,
                Note = m.Note,
                HomeScore = m.HomeScore,
                AwayScore = m.AwayScore,
                HomeTeam = new ResultTeamDto
                {
                    Id = m.HomeTeam!.Id,
                    Name = m.HomeTeam.Name,
                    Code = m.HomeTeam.Code,
                    City = m.HomeTeam.City,
                    Stadium = m.HomeTeam.Stadium,
                    Capacity = m.HomeTeam.Capacity,
                    ColorA = m.HomeTeam.ColorA,
                    ColorB = m.HomeTeam.ColorB,
                    Region = m.HomeTeam.Region,
                    Status = m.HomeTeam.Status
                },
                AwayTeam = new ResultTeamDto
                {
                    Id = m.AwayTeam!.Id,
                    Name = m.AwayTeam.Name,
                    Code = m.AwayTeam.Code,
                    City = m.AwayTeam.City,
                    Stadium = m.AwayTeam.Stadium,
                    Capacity = m.AwayTeam.Capacity,
                    ColorA = m.AwayTeam.ColorA,
                    ColorB = m.AwayTeam.ColorB,
                    Region = m.AwayTeam.Region,
                    Status = m.AwayTeam.Status
                },
                Goals = m.Goals.OrderBy(g => g.Minute).Select(g => new GoalItemDto
                {
                    Id = g.Id,
                    MatchId = g.MatchId,
                    TeamId = g.TeamId,
                    TeamName = g.Team?.Name ?? "",
                    PlayerName = g.PlayerName,
                    Minute = g.Minute,
                    IsPenalty = g.IsPenalty,
                    IsOwnGoal = g.IsOwnGoal,
                    AssistPlayerName = g.AssistPlayerName
                }).ToList(),
                Cards = m.Cards.OrderBy(c => c.Minute).Select(c => new CardItemDto
                {
                    Id = c.Id,
                    MatchId = c.MatchId,
                    TeamId = c.TeamId,
                    TeamName = c.Team?.Name ?? "",
                    PlayerName = c.PlayerName,
                    Minute = c.Minute,
                    CardType = c.CardType
                }).ToList(),
                Substitutions = m.Substitutions.OrderBy(s => s.Minute).Select(s => new SubstitutionItemDto
                {
                    Id = s.Id,
                    MatchId = s.MatchId,
                    TeamId = s.TeamId,
                    TeamName = s.Team?.Name ?? "",
                    PlayerIn = s.PlayerIn,
                    PlayerOut = s.PlayerOut,
                    Minute = s.Minute
                }).ToList(),
                Statistic = m.Statistic == null ? null : new MatchStatisticDto
                {
                    HomePossession = m.Statistic.HomePossession,
                    AwayPossession = m.Statistic.AwayPossession,
                    HomeShots = m.Statistic.HomeShots,
                    AwayShots = m.Statistic.AwayShots,
                    HomeShotsOnTarget = m.Statistic.HomeShotsOnTarget,
                    AwayShotsOnTarget = m.Statistic.AwayShotsOnTarget,
                    HomeCorners = m.Statistic.HomeCorners,
                    AwayCorners = m.Statistic.AwayCorners,
                    HomeFouls = m.Statistic.HomeFouls,
                    AwayFouls = m.Statistic.AwayFouls,
                    HomeOffsides = m.Statistic.HomeOffsides,
                    AwayOffsides = m.Statistic.AwayOffsides
                }
            };

            return Ok(detail);
        }

        // PUT: /api/matches/{id}/statistics  (yoksa oluşturur, varsa günceller)
        [HttpPut("{id}/statistics")]
        public async Task<IActionResult> SaveStatistic(int id, [FromBody] MatchStatisticDto dto)
        {
            var match = await _context.Matches.Include(m => m.Statistic).FirstOrDefaultAsync(m => m.Id == id);
            if (match == null) return NotFound(new { message = $"Maç #{id} bulunamadı." });

            if (match.Status != "Live" && match.Status != "Finished")
                return BadRequest(new { message = "İstatistik yalnızca canlı veya tamamlanan maçlara girilebilir." });

            var error = ValidateStatistic(dto, match.HomeScore ?? 0, match.AwayScore ?? 0);
            if (error != null) return BadRequest(new { message = error });

            var stat = match.Statistic ?? new MatchStatistic { MatchId = id };
            stat.HomePossession = dto.HomePossession;
            stat.AwayPossession = dto.AwayPossession;
            stat.HomeShots = dto.HomeShots;
            stat.AwayShots = dto.AwayShots;
            stat.HomeShotsOnTarget = dto.HomeShotsOnTarget;
            stat.AwayShotsOnTarget = dto.AwayShotsOnTarget;
            stat.HomeCorners = dto.HomeCorners;
            stat.AwayCorners = dto.AwayCorners;
            stat.HomeFouls = dto.HomeFouls;
            stat.AwayFouls = dto.AwayFouls;
            stat.HomeOffsides = dto.HomeOffsides;
            stat.AwayOffsides = dto.AwayOffsides;

            if (match.Statistic == null) _context.MatchStatistics.Add(stat);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(match.Id, match.Week, "updated");

            return NoContent();
        }

        // POST: /api/matches
        [HttpPost]
        public async Task<ActionResult<ResultMatchDto>> CreateMatch([FromBody] CreateMatchDto dto)
        {
            // Business Rule 1: Home and away team cannot be the same
            if (dto.HomeTeamId == dto.AwayTeamId)
            {
                return BadRequest(new { message = "Bir maçta ev sahibi ve deplasman takımı aynı olamaz." });
            }

            var error = ValidateMatch(dto.Week, dto.Status, dto.HomeScore, dto.AwayScore);
            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            var status = string.IsNullOrWhiteSpace(dto.Status) ? "NotStarted" : dto.Status;
            var hasScore = status == "Live" || status == "Finished";

            var homeTeam = await _context.Teams.FindAsync(dto.HomeTeamId);
            var awayTeam = await _context.Teams.FindAsync(dto.AwayTeamId);
            if (homeTeam == null || awayTeam == null)
            {
                return BadRequest(new { message = "Seçilen takımlardan biri veya her ikisi sistemde bulunamadı." });
            }

            // Business Rule 2: A team cannot play more than one match in the same week
            var existingMatch = await _context.Matches.AnyAsync(m =>
                m.Week == dto.Week &&
                (m.HomeTeamId == dto.HomeTeamId || m.AwayTeamId == dto.HomeTeamId ||
                 m.HomeTeamId == dto.AwayTeamId || m.AwayTeamId == dto.AwayTeamId));

            if (existingMatch)
            {
                return BadRequest(new { message = "Bir takım aynı hafta içerisinde birden fazla maçta yer alamaz." });
            }

            var match = new Match
            {
                Week = dto.Week,
                MatchDate = dto.MatchDate,
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId,
                HomeScore = hasScore ? dto.HomeScore : null,
                AwayScore = hasScore ? dto.AwayScore : null,
                Status = status,
                Stadium = !string.IsNullOrWhiteSpace(dto.Stadium) ? dto.Stadium.Trim() : homeTeam.Stadium,
                Referee = dto.Referee?.Trim(),
                Attendance = dto.Attendance,
                Note = dto.Note?.Trim()
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(match.Id, match.Week, "created");

            return CreatedAtAction(nameof(GetMatchDetail), new { id = match.Id }, new ResultMatchDto
            {
                Id = match.Id,
                Week = match.Week,
                MatchDate = match.MatchDate,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = homeTeam.Name,
                HomeTeamCode = homeTeam.Code,
                HomeColorA = homeTeam.ColorA,
                HomeColorB = homeTeam.ColorB,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = awayTeam.Name,
                AwayTeamCode = awayTeam.Code,
                AwayColorA = awayTeam.ColorA,
                AwayColorB = awayTeam.ColorB,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                Status = match.Status,
                Stadium = match.Stadium,
                Referee = match.Referee,
                Attendance = match.Attendance,
                Note = match.Note
            });
        }

        // PUT: /api/matches/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(int id, [FromBody] UpdateMatchDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "Kimlik bilgisi eşleşmiyor." });
            }

            // Business Rule 1: Home and away team cannot be the same
            if (dto.HomeTeamId == dto.AwayTeamId)
            {
                return BadRequest(new { message = "Bir maçta ev sahibi ve deplasman takımı aynı olamaz." });
            }

            var error = ValidateMatch(dto.Week, dto.Status, dto.HomeScore, dto.AwayScore);
            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            var hasScore = dto.Status == "Live" || dto.Status == "Finished";

            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound(new { message = $"Maç #{id} bulunamadı." });
            }

            var homeTeam = await _context.Teams.FindAsync(dto.HomeTeamId);
            var awayTeam = await _context.Teams.FindAsync(dto.AwayTeamId);
            if (homeTeam == null || awayTeam == null)
            {
                return BadRequest(new { message = "Seçilen takımlardan biri veya her ikisi geçerli değil." });
            }

            // Business Rule 2: A team cannot play more than once in the same week (excluding this match)
            var conflict = await _context.Matches.AnyAsync(m =>
                m.Id != id &&
                m.Week == dto.Week &&
                (m.HomeTeamId == dto.HomeTeamId || m.AwayTeamId == dto.HomeTeamId ||
                 m.HomeTeamId == dto.AwayTeamId || m.AwayTeamId == dto.AwayTeamId));

            if (conflict)
            {
                return BadRequest(new { message = "Bir takım aynı hafta içerisinde birden fazla maçta yer alamaz." });
            }

            // Gol animasyonu için: takımlar aynı kaldıysa hangi tarafın skoru arttı?
            var oldHomeScore = match.HomeScore ?? 0;
            var oldAwayScore = match.AwayScore ?? 0;
            var sameTeams = match.HomeTeamId == dto.HomeTeamId && match.AwayTeamId == dto.AwayTeamId;

            match.Week = dto.Week;
            match.MatchDate = dto.MatchDate;
            match.HomeTeamId = dto.HomeTeamId;
            match.AwayTeamId = dto.AwayTeamId;
            match.HomeScore = hasScore ? dto.HomeScore : null;
            match.AwayScore = hasScore ? dto.AwayScore : null;
            match.Status = dto.Status;
            match.Stadium = !string.IsNullOrWhiteSpace(dto.Stadium) ? dto.Stadium.Trim() : homeTeam.Stadium;
            match.Referee = dto.Referee?.Trim();
            match.Attendance = dto.Attendance;
            match.Note = dto.Note?.Trim();

            await _context.SaveChangesAsync();

            await NotifyMatchChangedAsync(match.Id, match.Week, "updated");
            if (sameTeams && (match.HomeScore ?? 0) > oldHomeScore) await NotifyGoalAsync(match, homeTeam, awayTeam, "home");
            if (sameTeams && (match.AwayScore ?? 0) > oldAwayScore) await NotifyGoalAsync(match, homeTeam, awayTeam, "away");

            return NoContent();
        }

        // DELETE: /api/matches/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound(new { message = $"Maç #{id} bulunamadı." });
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(match.Id, match.Week, "deleted");
            return NoContent();
        }

        // Event Management
        // POST: /api/matches/{id}/goals
        [HttpPost("{id}/goals")]
        public async Task<IActionResult> AddGoal(int id, [FromBody] GoalItemDto dto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return NotFound(new { message = $"Maç #{id} bulunamadı." });

            if (dto.TeamId != match.HomeTeamId && dto.TeamId != match.AwayTeamId)
            {
                return BadRequest(new { message = "Gol atılan takım maçtaki ev sahibi veya deplasman takımı olmalıdır." });
            }

            var error = ValidateEvent(dto.Minute, dto.PlayerName);
            if (error != null) return BadRequest(new { message = error });

            var goal = new MatchGoal
            {
                MatchId = id,
                TeamId = dto.TeamId,
                PlayerName = dto.PlayerName.Trim(),
                Minute = dto.Minute,
                IsPenalty = dto.IsPenalty,
                IsOwnGoal = dto.IsOwnGoal,
                AssistPlayerName = dto.AssistPlayerName?.Trim()
            };

            _context.MatchGoals.Add(goal);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(id, match.Week, "updated");

            return Ok(new { message = "Gol başarıyla eklendi.", goalId = goal.Id });
        }

        [HttpDelete("goals/{goalId}")]
        public async Task<IActionResult> DeleteGoal(int goalId)
        {
            var goal = await _context.MatchGoals.FindAsync(goalId);
            if (goal == null) return NotFound(new { message = $"Gol #{goalId} bulunamadı." });

            _context.MatchGoals.Remove(goal);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(goal.MatchId, 0, "updated");
            return NoContent();
        }

        // POST: /api/matches/{id}/cards
        [HttpPost("{id}/cards")]
        public async Task<IActionResult> AddCard(int id, [FromBody] CardItemDto dto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return NotFound(new { message = $"Maç #{id} bulunamadı." });

            if (dto.TeamId != match.HomeTeamId && dto.TeamId != match.AwayTeamId)
            {
                return BadRequest(new { message = "Kart gören takım maçın takımlarından biri olmalıdır." });
            }

            var error = ValidateEvent(dto.Minute, dto.PlayerName);
            if (error != null) return BadRequest(new { message = error });

            if (dto.CardType != "Yellow" && dto.CardType != "Red")
                return BadRequest(new { message = "Kart türü Yellow veya Red olmalıdır." });

            var card = new MatchCard
            {
                MatchId = id,
                TeamId = dto.TeamId,
                PlayerName = dto.PlayerName.Trim(),
                Minute = dto.Minute,
                CardType = dto.CardType
            };

            _context.MatchCards.Add(card);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(id, match.Week, "updated");

            return Ok(new { message = "Kart başarıyla eklendi.", cardId = card.Id });
        }

        [HttpDelete("cards/{cardId}")]
        public async Task<IActionResult> DeleteCard(int cardId)
        {
            var card = await _context.MatchCards.FindAsync(cardId);
            if (card == null) return NotFound(new { message = $"Kart #{cardId} bulunamadı." });

            _context.MatchCards.Remove(card);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(card.MatchId, 0, "updated");
            return NoContent();
        }

        // POST: /api/matches/{id}/substitutions
        [HttpPost("{id}/substitutions")]
        public async Task<IActionResult> AddSubstitution(int id, [FromBody] SubstitutionItemDto dto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return NotFound(new { message = $"Maç #{id} bulunamadı." });

            if (dto.TeamId != match.HomeTeamId && dto.TeamId != match.AwayTeamId)
            {
                return BadRequest(new { message = "Oyuncu değişikliği yapan takım maç takımlarından biri olmalıdır." });
            }

            var error = ValidateEvent(dto.Minute, dto.PlayerIn);
            if (error != null) return BadRequest(new { message = error });

            if (string.IsNullOrWhiteSpace(dto.PlayerOut))
                return BadRequest(new { message = "Oyundan çıkan oyuncu adı boş bırakılamaz." });

            if (dto.PlayerIn.Trim() == dto.PlayerOut.Trim())
                return BadRequest(new { message = "Giren ve çıkan oyuncu aynı kişi olamaz." });

            var sub = new MatchSubstitution
            {
                MatchId = id,
                TeamId = dto.TeamId,
                PlayerIn = dto.PlayerIn.Trim(),
                PlayerOut = dto.PlayerOut.Trim(),
                Minute = dto.Minute
            };

            _context.MatchSubstitutions.Add(sub);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(id, match.Week, "updated");

            return Ok(new { message = "Oyuncu değişikliği kaydedildi.", subId = sub.Id });
        }

        [HttpDelete("substitutions/{subId}")]
        public async Task<IActionResult> DeleteSubstitution(int subId)
        {
            var sub = await _context.MatchSubstitutions.FindAsync(subId);
            if (sub == null) return NotFound(new { message = $"Değişiklik #{subId} bulunamadı." });

            _context.MatchSubstitutions.Remove(sub);
            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(sub.MatchId, 0, "updated");
            return NoContent();
        }

        // POST: /api/matches/randomize-scores
        [HttpPost("randomize-scores")]
        public async Task<IActionResult> RandomizeScores([FromQuery] int? week, [FromQuery] bool forceAll = false)
        {
            var query = _context.Matches
                .Include(m => m.Goals)
                .Include(m => m.Cards)
                .Include(m => m.Statistic)
                .AsQueryable();

            if (week.HasValue)
            {
                query = query.Where(m => m.Week == week.Value);
            }

            if (!forceAll)
            {
                query = query.Where(m => m.Status != "Finished");
            }

            var matchesToRandomize = await query.ToListAsync();
            if (!matchesToRandomize.Any())
            {
                return Ok(new { message = "Skor atanacak uygun maç bulunamadı veya tüm maçlar zaten tamamlanmış.", count = 0 });
            }

            var rng = new Random();

            foreach (var match in matchesToRandomize)
            {
                match.HomeScore = rng.Next(0, 100) switch { < 25 => 0, < 60 => 1, < 85 => 2, < 96 => 3, _ => 4 };
                match.AwayScore = rng.Next(0, 100) switch { < 35 => 0, < 72 => 1, < 92 => 2, _ => 3 };
                match.Status = "Finished";
                match.Note = null;
                match.Attendance ??= rng.Next(15000, 68000);

                // Eski olayları silip yeni skora uygun gol, kart ve istatistik üret
                _context.MatchGoals.RemoveRange(match.Goals);
                _context.MatchCards.RemoveRange(match.Cards);
                if (match.Statistic != null) _context.MatchStatistics.Remove(match.Statistic);

                _context.MatchGoals.AddRange(MatchDataGenerator.BuildGoals(match));
                _context.MatchCards.AddRange(MatchDataGenerator.BuildCards(match));
                _context.MatchStatistics.Add(MatchDataGenerator.BuildStatistic(match));
            }

            await _context.SaveChangesAsync();
            await NotifyMatchChangedAsync(0, week ?? 0, "bulk");

            return Ok(new
            {
                message = $"{matchesToRandomize.Count} maç için rastgele skorlar, goller ve istatistikler oluşturuldu.",
                count = matchesToRandomize.Count
            });
        }

        // ---- Canlı yayın (SignalR) ----
        private Task NotifyMatchChangedAsync(int matchId, int week, string kind) =>
            _hub.Clients.All.SendAsync("MatchChanged", new { matchId, week, kind });

        private Task NotifyGoalAsync(Match match, Team home, Team away, string side) =>
            _hub.Clients.All.SendAsync("GoalScored", new
            {
                matchId = match.Id,
                week = match.Week,
                side,
                status = match.Status,
                homeScore = match.HomeScore ?? 0,
                awayScore = match.AwayScore ?? 0,
                home = new { name = home.Name, code = home.Code, colorA = home.ColorA, colorB = home.ColorB },
                away = new { name = away.Name, code = away.Code, colorA = away.ColorA, colorB = away.ColorB }
            });

        private static readonly string[] ValidStatuses = { "NotStarted", "Live", "Finished", "Postponed" };

        private static string? ValidateStatistic(MatchStatisticDto s, int homeScore, int awayScore)
        {
            var values = new[] { s.HomePossession, s.AwayPossession, s.HomeShots, s.AwayShots, s.HomeShotsOnTarget, s.AwayShotsOnTarget,
                                 s.HomeCorners, s.AwayCorners, s.HomeFouls, s.AwayFouls, s.HomeOffsides, s.AwayOffsides };
            if (values.Any(v => v < 0))
                return "İstatistik değerleri negatif olamaz.";

            if (s.HomePossession + s.AwayPossession != 100)
                return "Topa sahip olma yüzdeleri toplamı 100 olmalıdır.";

            if (s.HomeShotsOnTarget > s.HomeShots || s.AwayShotsOnTarget > s.AwayShots)
                return "İsabetli şut sayısı toplam şut sayısından fazla olamaz.";

            if (s.HomeShotsOnTarget < homeScore || s.AwayShotsOnTarget < awayScore)
                return "İsabetli şut sayısı atılan gol sayısından az olamaz.";

            return null;
        }

        private static string? ValidateEvent(int minute, string? playerName)
        {
            if (minute < 1 || minute > 120)
                return "Dakika 1 ile 120 arasında olmalıdır.";

            if (string.IsNullOrWhiteSpace(playerName))
                return "Oyuncu adı boş bırakılamaz.";

            return null;
        }

        private static string? ValidateMatch(int week, string? status, int? homeScore, int? awayScore)
        {
            if (week < 1 || week > 38)
                return "Hafta 1 ile 38 arasında olmalıdır.";

            if (!string.IsNullOrWhiteSpace(status) && !ValidStatuses.Contains(status))
                return "Geçersiz maç durumu. Geçerli değerler: NotStarted, Live, Finished, Postponed.";

            if (status == "Live" || status == "Finished")
            {
                if (homeScore == null || awayScore == null)
                    return "Canlı veya tamamlanan maçlarda iki takımın skoru da girilmelidir.";

                if (homeScore < 0 || awayScore < 0)
                    return "Skor negatif olamaz.";
            }

            return null;
        }
    }
}
