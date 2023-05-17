using MusicStoreMVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStoreMVCApp.Controllers
{
    public class HomeController : Controller
    {
        readonly MusicStoreAppEntities db = new MusicStoreAppEntities();

        public ActionResult Index()
        {
            List<Movie> movies = db.Movies.ToList();
            List<Genre> genres = db.Genres.ToList();
            List<MovieGenre> movieGenres = db.MovieGenres.ToList();
            List<MovieIdGenreIdModel> movieIdGenreIds = new List<MovieIdGenreIdModel>();

            foreach (MovieGenre movieGenre in movieGenres)
            {
                movieIdGenreIds.Add(new MovieIdGenreIdModel()
                {
                    MovieId = movieGenre.MovieId,
                    GenreId = movieGenre.GenreId
                });
            }

            GuestCustomerIndexViewModel model = new GuestCustomerIndexViewModel()
            {
                Movies = movies,
                Genres = genres,
                MovieIdGenreIds = movieIdGenreIds
            };

            return View(model);
        }
    }
}