using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademy_ApiProject.Context;
using MyAcademy_ApiProject.Dtos.TeamDtos;
using MyAcademy_ApiProject.Entities;

namespace MyAcademy_ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController(ApiContext _context) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultTeamDto>>> GetTeams([FromQuery] string? region, [FromQuery] string? status)
        {
            var query = _context.Teams.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(t => t.Region.ToLower() == region.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => t.Status.ToLower() == status.ToLower());
            }

            var teams = await query
                .OrderBy(t => t.Name)
                .Select(t => new ResultTeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Code = t.Code,
                    LogoUrl = t.LogoUrl,
                    City = t.City,
                    Stadium = t.Stadium,
                    Capacity = t.Capacity,
                    Founded = t.Founded,
                    ColorA = t.ColorA,
                    ColorB = t.ColorB,
                    Region = t.Region,
                    Status = t.Status
                })
                .ToListAsync();

            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultTeamDto>> GetTeam(int id)
        {
            var t = await _context.Teams.FindAsync(id);
            if (t == null)
            {
                return NotFound(new { message = $"Takım #{id} bulunamadı." });
            }

            return Ok(new ResultTeamDto
            {
                Id = t.Id,
                Name = t.Name,
                Code = t.Code,
                LogoUrl = t.LogoUrl,
                City = t.City,
                Stadium = t.Stadium,
                Capacity = t.Capacity,
                Founded = t.Founded,
                ColorA = t.ColorA,
                ColorB = t.ColorB,
                Region = t.Region,
                Status = t.Status
            });
        }

        [HttpPost]
        public async Task<ActionResult<ResultTeamDto>> CreateTeam([FromBody] CreateTeamDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { message = "Takım adı boş bırakılamaz." });
            }

            var team = new Team
            {
                Name = dto.Name.Trim(),
                Code = string.IsNullOrWhiteSpace(dto.Code) ? dto.Name.Substring(0, Math.Min(3, dto.Name.Length)).ToUpper() : dto.Code.Trim().ToUpper(),
                LogoUrl = dto.LogoUrl,
                City = dto.City?.Trim() ?? string.Empty,
                Stadium = dto.Stadium?.Trim() ?? string.Empty,
                Capacity = dto.Capacity,
                Founded = dto.Founded,
                ColorA = string.IsNullOrWhiteSpace(dto.ColorA) ? "#1E5BD8" : dto.ColorA,
                ColorB = string.IsNullOrWhiteSpace(dto.ColorB) ? "#FFFFFF" : dto.ColorB,
                Region = string.IsNullOrWhiteSpace(dto.Region) ? "north" : dto.Region.ToLower(),
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "active" : dto.Status.ToLower()
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, new ResultTeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Code = team.Code,
                LogoUrl = team.LogoUrl,
                City = team.City,
                Stadium = team.Stadium,
                Capacity = team.Capacity,
                Founded = team.Founded,
                ColorA = team.ColorA,
                ColorB = team.ColorB,
                Region = team.Region,
                Status = team.Status
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] UpdateTeamDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "Kimlik bilgisi eşleşmiyor." });
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound(new { message = $"Takım #{id} bulunamadı." });
            }

            team.Name = dto.Name.Trim();
            team.Code = dto.Code.Trim().ToUpper();
            team.LogoUrl = dto.LogoUrl;
            team.City = dto.City.Trim();
            team.Stadium = dto.Stadium.Trim();
            team.Capacity = dto.Capacity;
            team.Founded = dto.Founded;
            team.ColorA = dto.ColorA;
            team.ColorB = dto.ColorB;
            team.Region = dto.Region.ToLower();
            team.Status = dto.Status.ToLower();

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound(new { message = $"Takım #{id} bulunamadı." });
            }

            // Check if team is in any match
            var hasMatches = await _context.Matches.AnyAsync(m => m.HomeTeamId == id || m.AwayTeamId == id);
            if (hasMatches)
            {
                // Soft delete by setting status to inactive
                team.Status = "inactive";
                await _context.SaveChangesAsync();
                return Ok(new { message = "Takım maç geçmişi olduğu için pasife alındı." });
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
