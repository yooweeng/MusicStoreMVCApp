using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStoreMVCApp.Controllers
{
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
            approvalItemById.Status = 0;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}