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
            int currentUserId = int.Parse(User.Identity.GetUserId());
            int currentCustomerId = db.Customers.Where(customer => customer.UserId == currentUserId).First().CustomerId;

            List<Cart> cartToShow = db.Carts.Where(cartItem => cartItem.CustomerId == currentCustomerId)
                                            .OrderBy(cart => cart.MovieId).ToList();
            return View(cartToShow);
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

        [HttpPost]
        public JsonResult RemoveCart(int movieId)
        {
            bool status = false;
            string statusMessage = "Failed to remove item from the cart";

            Cart cartItemByMovieId = db.Carts.Where(cart => cart.MovieId == movieId).FirstOrDefault();

            if(cartItemByMovieId != null)
            {
                db.Carts.Remove(cartItemByMovieId);
                db.SaveChanges();
                status = true;
                statusMessage = "Successfully remove item from the cart";
            }

            return Json(new { Status = status, StatusMessage = statusMessage });
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

        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Checkout(List<int> selectedMovieIds)
        {
            bool status = false;
            string statusMessage = "Failed to remove item from the cart";

            if(selectedMovieIds != null)
            {
                int currentUserId = int.Parse(User.Identity.GetUserId());
                int currentCustomerId = db.Customers.Where(customer => customer.UserId == currentUserId).First().CustomerId;
                int quantity;

                // create a new order
                Order order = db.Orders.Add(new Order() { Date = DateTime.Now, Status = "pending", ReferenceNumber = "test" });
                db.SaveChanges();

                foreach (int movieId in selectedMovieIds)
                {
                    Movie movie = db.Movies.Where(m => m.Id == movieId).Single();
                    List<Cart> cartItemsByMovieId = db.Carts.Where(cartItem => (cartItem.MovieId == movieId) && 
                                                                                (cartItem.CustomerId == currentCustomerId)).ToList();
                    quantity = cartItemsByMovieId.Count;

                    // remove from cart
                    foreach(Cart cartItem in cartItemsByMovieId)
                    {
                        db.Carts.Remove(cartItem);
                    }

                    db.OrderMovies.Add(new OrderMovie()
                    {
                        MovieId = movieId,
                        UnitPrice = movie.Price,
                        Quantity = quantity,
                        Order = order
                    });
                }
                db.SaveChanges();
            }

            return Json(new { Status = status, StatusMessage = statusMessage });
        }
    }
}