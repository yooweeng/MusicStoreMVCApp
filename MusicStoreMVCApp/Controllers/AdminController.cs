using MusicStoreMVCApp.Models;
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
            List<ApprovalList> approvalList = db.ApprovalLists.ToList();
            List<AdminIndexApprovalListViewModel> adminIndexApprovalList = new List<AdminIndexApprovalListViewModel>();

            foreach(ApprovalList approvalListItem in approvalList)
            {
                adminIndexApprovalList.Add(new AdminIndexApprovalListViewModel
                {
                    SellerEmail = approvalListItem.SellerEmail,
                    SellerFname = approvalListItem.SellerFname,
                    SellerLname = approvalListItem.SellerLname,
                    Address = approvalListItem.Address,
                    PhoneNumber = approvalListItem.PhoneNumber,
                    Status = approvalListItem.Status
                });
            }

            return View(adminIndexApprovalList);
        }

        public JsonResult Approve(int Id)
        {
            // update approval list status
            ApprovalList approvalItemById = db.ApprovalLists.SingleOrDefault(item => item.Id == Id);
            approvalItemById.Status = 1;

            // insert record into seller table
            db.Sellers.Add(new Seller()
            {
                Fname = approvalItemById.SellerFname,
                Lname = approvalItemById.SellerLname,
                Address = approvalItemById.Address,
                PhoneNumber = approvalItemById.PhoneNumber
            });

            db.SaveChanges();

            return Json(new { 
                Status = true,
                StatusMessage = "Successfully updated the status of selected seller account to approve"
            });
        }

        public JsonResult Reject(int Id)
        {
            ApprovalList approvalItemById = db.ApprovalLists.SingleOrDefault(item => item.Id == Id);
            approvalItemById.Status = 2;
            db.SaveChanges();

            return Json(new
            {
                Status = true,
                StatusMessage = "Successfully updated the status of selected seller account to reject"
            });
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