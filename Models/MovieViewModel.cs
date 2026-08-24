using System.Text.Json.Serialization;

namespace WebApplication11.Models
{
    public class MovieViewModel
    {
        public string SearchQuery { get; set; } = string.Empty;
        public MovieEntity? CurrentMovie { get; set; }
        public List<MovieEntity> RecentMovies { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class OmdbMovieResponse
    {
        [JsonPropertyName("Title")] public string Title { get; set; } = string.Empty;
        [JsonPropertyName("Year")] public string Year { get; set; } = string.Empty;
        [JsonPropertyName("Poster")] public string Poster { get; set; } = string.Empty;
        [JsonPropertyName("imdbRating")] public string ImdbRating { get; set; } = string.Empty;
        [JsonPropertyName("Runtime")] public string Runtime { get; set; } = string.Empty;
        [JsonPropertyName("Director")] public string Director { get; set; } = string.Empty;
        [JsonPropertyName("Actors")] public string Actors { get; set; } = string.Empty;
        [JsonPropertyName("Plot")] public string Plot { get; set; } = string.Empty;
        [JsonPropertyName("Response")] public string Response { get; set; } = string.Empty;
        [JsonPropertyName("Error")] public string? Error { get; set; }
    }
}