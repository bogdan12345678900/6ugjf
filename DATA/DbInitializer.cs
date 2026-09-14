using WebApplication11.Models;

namespace WebApplication11.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Movies.Any(m => m.IsFavorite))
            {
                return; // База даних вже містить улюблені фільми
            }

            var favorites = new MovieEntity[]
            {
                new MovieEntity
                {
                    Title = "Inception",
                    Year = "2010",
                    Poster = "https://m.media-amazon.com/images/M/MVBMTM0MDY1MTM5N15BMl5BanBnXkFtZTcwOTA3MTE3Mw@@._V1_SX300.jpg",
                    ImdbRating = "8.8",
                    Runtime = "148 min",
                    Director = "Christopher Nolan",
                    Actors = "Leonardo DiCaprio, Joseph Gordon-Levitt, Elliot Page",
                    Plot = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                    IsFavorite = true
                },
                new MovieEntity
                {
                    Title = "The Dark Knight",
                    Year = "2008",
                    Poster = "https://m.media-amazon.com/images/M/MVBMTMxMDM1NjA3Ml5BMl5BanBnXkFtZTcwNTMyMTA3Mw@@._V1_SX300.jpg",
                    ImdbRating = "9.0",
                    Runtime = "152 min",
                    Director = "Christopher Nolan",
                    Actors = "Christian Bale, Heath Ledger, Aaron Eckhart",
                    Plot = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                    IsFavorite = true
                },
                new MovieEntity
                {
                    Title = "Interstellar",
                    Year = "2014",
                    Poster = "https://m.media-amazon.com/images/M/MVBMTYzMDU2Mzg4NV5BMl5BanBnXkFtZTgwMDg4MjI3MjE@._V1_SX300.jpg",
                    ImdbRating = "8.7",
                    Runtime = "169 min",
                    Director = "Christopher Nolan",
                    Actors = "Matthew McConaughey, Anne Hathaway, Jessica Chastain",
                    Plot = "When Earth becomes uninhabitable in the future, a farmer and ex-NASA pilot, Joseph Cooper, is tasked to pilot a spacecraft, along with a team of researchers, to find a new planet for humans.",
                    IsFavorite = true
                },
                new MovieEntity
                {
                    Title = "The Matrix",
                    Year = "1999",
                    Poster = "https://m.media-amazon.com/images/M/MVBBNzQ2MDM2OTUxNV5BMl5BanBnXkFtZTgwNTM3MTEyMDE@._V1_SX300.jpg",
                    ImdbRating = "8.7",
                    Runtime = "136 min",
                    Director = "Lana Wachowski, Lilly Wachowski",
                    Actors = "Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss",
                    Plot = "When a beautiful stranger leads computer hacker Neo to a forbidding underworld, he discovers the shocking truth--the life he knows is the elaborate deception of an evil cyber-intelligence.",
                    IsFavorite = true
                },
                new MovieEntity
                {
                    Title = "Pulp Fiction",
                    Year = "1994",
                    Poster = "https://m.media-amazon.com/images/M/MVBMTkxMTA5MDk1NF5BMl5BanBnXkFtZTgwMjg5NjU3MDE@._V1_SX300.jpg",
                    ImdbRating = "8.9",
                    Runtime = "154 min",
                    Director = "Quentin Tarantino",
                    Actors = "John Travolta, Uma Thurman, Samuel L. Jackson",
                    Plot = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                    IsFavorite = true
                },
                new MovieEntity
                {
                    Title = "Fight Club",
                    Year = "1999",
                    Poster = "https://m.media-amazon.com/images/M/MVBMTE5Njg1NDM2Ml5BMl5BanBnXkFtZTcwMjA3MTE3Mw@@._V1_SX300.jpg",
                    ImdbRating = "8.8",
                    Runtime = "139 min",
                    Director = "David Fincher",
                    Actors = "Brad Pitt, Edward Norton, Meat Loaf",
                    Plot = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.",
                    IsFavorite = true
                }
            };

            context.Movies.AddRange(favorites);
            context.SaveChanges();
        }
    }
}