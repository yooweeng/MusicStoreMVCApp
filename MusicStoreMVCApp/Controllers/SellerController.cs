using MusicStoreMVCApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStoreMVCApp.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        readonly MusicStoreAppEntities db = new MusicStoreAppEntities();

        // GET: Seller
        public ActionResult Index()
        {
            List<Movie> movies = db.Movies.ToList();

            return View(movies);
        }

        public ActionResult AddMovie()
        {
            List<Genre> genres = db.Genres.ToList();
            List<string> genreTypes = new List<string>();

            foreach(Genre genre in genres)
            {
                genreTypes.Add(genre.GenreType);
            }

            AddMovieViewModel model = new AddMovieViewModel() { GenreTypes = genreTypes };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddMovie(Movie movie, List<string> genreTypes)
        {
            foreach(var item in genreTypes)
            {
                Debug.WriteLine("genreSelet"+item);
            }
            // insert into Movie table
            //var insertedMovie = db.Movies.Add(new Movie()
            //{
            //    MovieTitle = movie.MovieTitle,
            //    Description = movie.Description,
            //    Price = movie.Price,
            //    ImageUrl = movie.ImageUrl,
            //    ReleasedYear = movie.ReleasedYear,
            //    SellerId = 1
            //});
            //db.SaveChanges();

            //foreach (GenreSelectedModel genreSelected in genreSelectedList)
            //{
            //    if(genreSelected.IsSelected)
            //    {
            //        // insert into MovieGenre table
            //        db.MovieGenres.Add(new MovieGenre()
            //        {
            //            MovieId = insertedMovie.Id,
            //            GenreId = genreSelected.Genre.Id
            //        });
            //    }
            //}
            //db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}