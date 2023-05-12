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

        [HttpPost]
        public JsonResult Movie(Movie movie, List<int> selectedGenresId, HttpPostedFileBase file)
        {
            string folderDirectory = "/MovieCover";
            string filename = "";

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
                    string path = Path.Combine(Server.MapPath("~" + folderDirectory + "/"),
                                               insertedMovie.Id.ToString());
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filename = Path.GetFileName(file.FileName);
                    path = Path.Combine(path, filename);
                    file.SaveAs(path);
                }
                catch (Exception ex)
                { Debug.WriteLine(ex); }
            }

            // update movie for imageUrl
            insertedMovie.ImageUrl = folderDirectory + "/" + insertedMovie.Id.ToString() + "/" + filename;

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

        [HttpDelete]
        public ActionResult Movie(int id)
        {
            List<MovieGenre> movieGenresByMovieId = db.MovieGenres.Where(movieGenre => movieGenre.MovieId == id).ToList();
            foreach (MovieGenre movieGenre in movieGenresByMovieId)
            {
                db.MovieGenres.Remove(movieGenre);
            }

            Movie movieById = db.Movies.Where(movie => movie.Id == id).FirstOrDefault();
            db.Movies.Remove(movieById);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult AddMovie()
        {
            List<Genre> genres = db.Genres.ToList();

            AddMovieViewModel model = new AddMovieViewModel() { Genres = genres };

            return View(model);
        }
    }
}