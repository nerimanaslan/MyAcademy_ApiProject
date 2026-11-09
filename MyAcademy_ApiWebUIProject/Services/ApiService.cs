using MyAcademy_ApiWebUIProject.ViewModels;
using System.Net.Http.Json;

namespace MyAcademy_ApiWebUIProject.Services
{
    public interface IApiService
    {
        Task<StandingsPageViewModel?> GetStandingsSummaryAsync(int? week = null);
        Task<StandingsHistoryViewModel> GetStandingsHistoryAsync();
        Task<List<TeamStandingViewModel>> GetStandingsAsync();
        Task<List<MatchViewModel>> GetMatchesAsync(int? week = null, string? team = null, string? status = null);
        Task<List<MatchViewModel>> GetMatchesByWeekAsync(int week);
        Task<MatchDetailViewModel?> GetMatchDetailAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateMatchAsync(object dto);
        Task<(bool Success, string? ErrorMessage)> UpdateMatchAsync(int id, object dto);
        Task<(bool Success, string? ErrorMessage)> DeleteMatchAsync(int id);
        Task<(bool Success, string? ErrorMessage)> SaveStatisticAsync(int matchId, MatchStatisticViewModel statistic);
        Task<(bool Success, string Message)> RandomizeScoresAsync(int? week = null, bool forceAll = false);

        Task<List<TeamViewModel>> GetTeamsAsync(string? region = null, string? status = null);
        Task<TeamViewModel?> GetTeamAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateTeamAsync(object dto);
        Task<(bool Success, string? ErrorMessage)> UpdateTeamAsync(int id, object dto);
        Task<(bool Success, string? ErrorMessage)> DeleteTeamAsync(int id);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<StandingsPageViewModel?> GetStandingsSummaryAsync(int? week = null)
        {
            try
            {
                var response = await _httpClient.GetAsync(week.HasValue ? $"standings/summary?week={week}" : "standings/summary");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;   // o haftada tamamlanmış maç yok
                return await response.Content.ReadFromJsonAsync<StandingsPageViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching standings summary.");
                return new StandingsPageViewModel();
            }
        }

        public async Task<StandingsHistoryViewModel> GetStandingsHistoryAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<StandingsHistoryViewModel>("standings/history") ?? new StandingsHistoryViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching standings history.");
                return new StandingsHistoryViewModel();
            }
        }

        public async Task<List<TeamStandingViewModel>> GetStandingsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<TeamStandingViewModel>>("standings");
                return result ?? new List<TeamStandingViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching standings.");
                return new List<TeamStandingViewModel>();
            }
        }

        public async Task<List<MatchViewModel>> GetMatchesAsync(int? week = null, string? team = null, string? status = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (week.HasValue) queryParams.Add($"week={week.Value}");
                if (!string.IsNullOrWhiteSpace(team)) queryParams.Add($"team={Uri.EscapeDataString(team)}");
                if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");

                var url = "matches" + (queryParams.Any() ? "?" + string.Join("&", queryParams) : "");
                var result = await _httpClient.GetFromJsonAsync<List<MatchViewModel>>(url);
                return result ?? new List<MatchViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching matches.");
                return new List<MatchViewModel>();
            }
        }

        public async Task<List<MatchViewModel>> GetMatchesByWeekAsync(int week)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<MatchViewModel>>($"matches/week/{week}");
                return result ?? new List<MatchViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching matches for week {week}.");
                return new List<MatchViewModel>();
            }
        }

        public async Task<MatchDetailViewModel?> GetMatchDetailAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MatchDetailViewModel>($"matches/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching match detail #{id}.");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateMatchAsync(object dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("matches", dto);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating match.");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateMatchAsync(int id, object dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"matches/{id}", dto);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating match #{id}.");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> SaveStatisticAsync(int matchId, MatchStatisticViewModel statistic)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"matches/{matchId}/statistics", statistic);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving statistics for match #{matchId}.");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteMatchAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"matches/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting match #{id}.");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> RandomizeScoresAsync(int? week = null, bool forceAll = false)
        {
            try
            {
                var queryParams = new List<string>();
                if (week.HasValue) queryParams.Add($"week={week.Value}");
                if (forceAll) queryParams.Add("forceAll=true");

                var url = "matches/randomize-scores" + (queryParams.Any() ? "?" + string.Join("&", queryParams) : "");
                var response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadFromJsonAsync<ApiMessageResponse>();
                    return (true, res?.Message ?? "Skorlar başarıyla oluşturuldu.");
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error randomizing match scores.");
                return (false, ex.Message);
            }
        }

        public async Task<List<TeamViewModel>> GetTeamsAsync(string? region = null, string? status = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(region)) queryParams.Add($"region={Uri.EscapeDataString(region)}");
                if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");

                var url = "teams" + (queryParams.Any() ? "?" + string.Join("&", queryParams) : "");
                var result = await _httpClient.GetFromJsonAsync<List<TeamViewModel>>(url);
                return result ?? new List<TeamViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams.");
                return new List<TeamViewModel>();
            }
        }

        public async Task<TeamViewModel?> GetTeamAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TeamViewModel>($"teams/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching team #{id}.");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateTeamAsync(object dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("teams", dto);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team.");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateTeamAsync(int id, object dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"teams/{id}", dto);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating team #{id}.");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteTeamAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"teams/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.Message ?? $"Sunucu hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting team #{id}.");
                return (false, ex.Message);
            }
        }

        private class ApiErrorResponse
        {
            public string? Message { get; set; }
        }

        private class ApiMessageResponse
        {
            public string? Message { get; set; }
        }
    }
}
