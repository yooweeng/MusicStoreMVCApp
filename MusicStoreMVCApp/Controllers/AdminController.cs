using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStoreMVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        MusicStoreAppEntities db = new MusicStoreAppEntities();

        // GET: Admin
        public ActionResult Index()
        {
            return View(db.ApprovalLists.ToList());
        }

        public ActionResult Approve(int Id)
        {
            ApprovalList approvalItemById = db.ApprovalLists.SingleOrDefault(item => item.Id == Id);
            approvalItemById.Status = 1;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Reject(int Id)
        {
            ApprovalList approvalItemById = db.ApprovalLists.SingleOrDefault(item => item.Id == Id);
            approvalItemById.Status = 2;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AddCategory()
        {
            List<Genre> genres = db.Genres.ToList();
            return View(genres);
        }

        [HttpPost]
        public ActionResult Genre(string GenreType)
        {
            db.Genres.Add(new Genre() { GenreType = GenreType });
            db.SaveChanges();

            return RedirectToAction("AddCategory");
        }
    }
}