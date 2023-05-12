using MusicStoreMVCApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            AddMovieViewModel model = new AddMovieViewModel() { Genres = genres };

            return View(model);
        }

        [HttpPost]
        public JsonResult AddMovie(Movie movie, List<int> selectedGenresId, HttpPostedFileBase file)
        {
            string path = "";
            // insert into Movie table without imageUrl
            var insertedMovie = db.Movies.Add(new Movie()
            {
                MovieTitle = movie.MovieTitle,
                Description = movie.Description,
                Price = movie.Price,
                ReleasedYear = movie.ReleasedYear,
                SellerId = 1
            });
            db.SaveChanges();

            // save movie cover
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Path.Combine(Server.MapPath("~/MovieCover/"),
                                               insertedMovie.Id.ToString());
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = Path.Combine(path, Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                }
                catch (Exception ex)
                { Debug.WriteLine(ex); }
            }

            // update movie for imageUrl
            insertedMovie.ImageUrl = path;

            // insert into MovieGenre table
            foreach (int genreId in selectedGenresId)
            {
                db.MovieGenres.Add(new MovieGenre()
                {
                    MovieId = insertedMovie.Id,
                    GenreId = genreId
                });
            }
            db.SaveChanges();

            return Json(new { StatusMessage = "", RedirectURL = "/Seller" });
        }
    }
}