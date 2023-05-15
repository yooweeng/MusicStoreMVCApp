using MusicStoreMVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStoreMVCApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        readonly MusicStoreAppEntities db = new MusicStoreAppEntities();

        // GET: Customer
        public ActionResult Index()
        {
            List<Movie> movies = db.Movies.ToList();
            List<Genre> genres = db.Genres.ToList();
            List<MovieGenre> movieGenres = db.MovieGenres.ToList();
            List<MovieIdGenreIdModel> movieIdGenreIds =new List<MovieIdGenreIdModel>();

            foreach(MovieGenre movieGenre in movieGenres)
            {
                movieIdGenreIds.Add(new MovieIdGenreIdModel()
                {
                    MovieId = movieGenre.MovieId,
                    GenreId = movieGenre.GenreId
                });
            }

            CustomerIndexViewModel model = new CustomerIndexViewModel() { 
                Movies = movies, Genres = genres, MovieIdGenreIds = movieIdGenreIds 
            };

            return View(model);
        }

        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult MovieDetail(int id)
        {
            Movie movieById = db.Movies.Where(movie => movie.Id == id).FirstOrDefault();

            if (movieById != null)
            {
                return View(movieById);
            }

            return View("Error");
        }
    }
}