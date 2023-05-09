using MusicStoreMVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStoreMVCApp.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        MusicStoreAppEntities db = new MusicStoreAppEntities();

        // GET: Seller
        public ActionResult Index()
        {
            List<Genre> genres = db.Genres.ToList();
            List<string> genreTypes = new List<string>();
            foreach(Genre genre in genres)
            {
                genreTypes.Add(genre.GenreType);
            }
            SellerIndexViewModel model = new SellerIndexViewModel() { GenreTypes = genreTypes };
            return View(model);
        }
    }
}