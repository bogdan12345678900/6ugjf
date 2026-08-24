using System.Net.Http.Json;
using WebApplication11.Models;

namespace WebApplication11.Services
{
    public interface IOmdbService
    {
        Task<OmdbMovieResponse?> GetMovieByTitleAsync(string title);
    }

    public class OmdbService : IOmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<OmdbMovieResponse?> GetMovieByTitleAsync(string title)
        {
            var apiKey = _configuration["OmdbSettings:ApiKey"];
            var baseUrl = _configuration["OmdbSettings:BaseUrl"] ?? "https://www.omdbapi.com/";

            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "d7d9c82b")
            {
                return new OmdbMovieResponse
                {
                    Response = "False",
                    Error = "Вкажіть дійсний OMDb API Key у файлі appsettings.json."
                };
            }

            var url = $"{baseUrl}?t={Uri.EscapeDataString(title)}&apikey={apiKey}&plot=full";

            try
            {
                return await _httpClient.GetFromJsonAsync<OmdbMovieResponse>(url);
            }
            catch
            {
                return new OmdbMovieResponse
                {
                    Response = "False",
                    Error = "Не вдалося отримати дані від сервера OMDb."
                };
            }
        }
    }
}