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

            GuestCustomerIndexViewModel model = new GuestCustomerIndexViewModel() { 
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

        public ActionResult Checkout(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        [HttpPost]
        public JsonResult Checkout(List<int> selectedMovieIds)
        {
            bool status = false;
            string statusMessage = "Failed to remove item from the cart";
            int orderId = 0;

            if(selectedMovieIds != null)
            {
                int currentUserId = int.Parse(User.Identity.GetUserId());
                int currentCustomerId = db.Customers.Where(customer => customer.UserId == currentUserId).First().CustomerId;
                int quantity;

                List<Movie> moviesBySelectedMovieId = new List<Movie>();
                Dictionary<int, Order> orderDict = new Dictionary<int, Order>();

                // get list of movie by the movieId
                foreach (int movieId in selectedMovieIds)
                {
                    Movie movie = db.Movies.Where(m => m.Id == movieId).Single();
                    moviesBySelectedMovieId.Add(movie);
                }
                // group the list of movie by sellerid, number of group here represent number of unique seller
                foreach (var group in moviesBySelectedMovieId.GroupBy(movie => movie.SellerId))
                {
                    // create new order
                    Order order = db.Orders.Add(new Order() { Date = DateTime.Now, Status = "pending", CustomerId = currentCustomerId });
                    // add the order to dictionary:
                    // - sellerid (key)
                    // - order (value)
                    orderDict.Add(group.Key, order);
                }
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

                    OrderMovie orderMovie = db.OrderMovies.Add(new OrderMovie()
                                            {
                                                MovieId = movieId,
                                                UnitPrice = movie.Price,
                                                Quantity = quantity,
                                                Order = orderDict[movie.SellerId]
                                            });
                    orderId = orderMovie.OrderId.HasValue ? orderMovie.OrderId.Value : 0;
                }
                db.SaveChanges();
            }

            return Json(new { Status = status, StatusMessage = statusMessage, OrderId = orderId });
        }

        public ActionResult OrderHistory(string orderStatus)
        {
            orderStatus = orderStatus == null ? "pending" : orderStatus;

            ViewBag.Status = orderStatus;

            int currentUserId = int.Parse(User.Identity.GetUserId());
            int currentCustomerId = db.Customers.Where(customer => customer.UserId == currentUserId).First().CustomerId;

            List<OrderMovie> orders = db.OrderMovies.Where(order => (order.Order.CustomerId == currentCustomerId) && 
                                                    (order.Order.Status == orderStatus))
                                                    .OrderByDescending(order => order.Order.Date)
                                                    .ThenByDescending(order => order.Order.Id).ToList();

            return View(orders);
        }
    }
}