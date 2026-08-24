using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication11.Data;
using WebApplication11.Models;
using WebApplication11.Services;

namespace WebApplication11.Controllers
{
    /// <summary>
    /// Головний контролер вебзастосунку «Кінопошук».
    /// Відповідає за відображення інтерфейсу користувача, виконання пошуку фільмів через OMDb API 
    /// та кешування/збереження історії пошуків у локальній базі даних SQLite.
    /// </summary>
    /// <remarks>
    /// Контролер використовує підхід Dependency Injection (DI) для отримання сервісу <see cref="IOmdbService"/> 
    /// та контексту бази даних <see cref="AppDbContext"/>.
    /// </remarks>
    public class HomeController : Controller
    {
        /// <summary>
        /// Сервіс для взаємодії з зовнішнім OMDb API.
        /// </summary>
        private readonly IOmdbService _omdbService;

        /// <summary>
        /// Контекст Entity Framework Core для роботи з базою даних SQLite.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="HomeController"/>.
        /// </summary>
        /// <param name="omdbService">Сервіс отримання даних про фільми з OMDb API.</param>
        /// <param name="context">Контекст бази даних SQLite.</param>
        /// <exception cref="ArgumentNullException">Виникає, якщо один із переданих сервісів є null.</exception>
        public HomeController(IOmdbService omdbService, AppDbContext context)
        {
            _omdbService = omdbService ?? throw new ArgumentNullException(nameof(omdbService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Обробляє HTTP GET запит на головну сторінку додатка.
        /// Завантажує останні 5 фільмів з історії пошуків SQLite БД.
        /// </summary>
        /// <returns>
        /// Об'єкт <see cref="IActionResult"/> з представленням сторінки Index 
        /// та моделлю <see cref="MovieViewModel"/>, що містить список останніх пошуків.
        /// </returns>
        /// <response code="200">Повертає головну сторінку з історією пошуків.</response>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new MovieViewModel
            {
                RecentMovies = await GetRecentMoviesAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Обробляє HTTP POST запит при надсиланні форми пошуку фільму.
        /// </summary>
        /// <param name="model">Модель <see cref="MovieViewModel"/> з введеною назвою фільму.</param>
        /// <returns>
        /// Об'єкт <see cref="IActionResult"/> з оновленими даними про фільм та оновленою історією пошуку.
        /// </returns>
        /// <remarks>
        /// Логіка виконання:
        /// 1. Перевіряє вхідний рядок на порожнечу.
        /// 2. Виконує асинхронний запит до OMDb API.
        /// 3. У разі успіху перевіряє наявність фільму у БД SQLite:
        ///    - Якщо фільм вже є — оновлює час пошуку <see cref="MovieEntity.SearchedAt"/>.
        ///    - Якщо фільм новий — зберігає новий запис у таблицю.
        /// 4. Повертає оновлену модель у Razor View.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Index(MovieViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SearchQuery))
            {
                model.ErrorMessage = "Будь ласка, введіть назву фільму.";
                model.RecentMovies = await GetRecentMoviesAsync();
                return View(model);
            }

            var apiResult = await _omdbService.GetMovieByTitleAsync(model.SearchQuery);

            if (apiResult != null && apiResult.Response == "True")
            {
                var existingMovie = await _context.Movies
                    .FirstOrDefaultAsync(m => m.Title.ToLower() == apiResult.Title.ToLower());

                if (existingMovie != null)
                {
                    existingMovie.SearchedAt = DateTime.UtcNow;
                    _context.Movies.Update(existingMovie);
                    model.CurrentMovie = existingMovie;
                }
                else
                {
                    var newEntity = new MovieEntity
                    {
                        Title = apiResult.Title,
                        Year = apiResult.Year,
                        Poster = apiResult.Poster,
                        ImdbRating = apiResult.ImdbRating,
                        Runtime = apiResult.Runtime,
                        Director = apiResult.Director,
                        Actors = apiResult.Actors,
                        Plot = apiResult.Plot,
                        SearchedAt = DateTime.UtcNow
                    };

                    _context.Movies.Add(newEntity);
                    model.CurrentMovie = newEntity;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                model.ErrorMessage = apiResult?.Error == "Movie not found!"
                    ? "Фільм не знайдено."
                    : apiResult?.Error ?? "Сталася помилка під час пошуку.";
            }

            model.RecentMovies = await GetRecentMoviesAsync();
            return View(model);
        }

        /// <summary>
        /// Приватний допоміжний метод для отримання останніх знайдених фільмів з SQLite.
        /// </summary>
        /// <returns>
        /// Асинхронна задача з результатом у вигляді списку <see cref="List{MovieEntity}"/>, 
        /// обмеженого 5 записами та відсортованого від найновіших.
        /// </returns>
        private Task<List<MovieEntity>> GetRecentMoviesAsync()
        {
            return _context.Movies
                .OrderByDescending(m => m.SearchedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}