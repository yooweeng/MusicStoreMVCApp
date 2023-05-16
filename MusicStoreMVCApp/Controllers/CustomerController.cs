using Microsoft.AspNet.Identity;
using MusicStoreMVCApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            return View(db.Carts.ToList());
        }

        [HttpPost]
        public JsonResult Cart(int movieId)
        {
            int currentUserId = int.Parse(User.Identity.GetUserId());
            int currentCustomerId = db.Customers.Where(customer => customer.UserId == currentUserId).First().CustomerId;

            db.Carts.Add(new Cart()
            {
                MovieId = movieId,
                CustomerId = currentCustomerId
            });
            db.SaveChanges();

            return Json(new { Status = true, StatusMessage = "" });
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