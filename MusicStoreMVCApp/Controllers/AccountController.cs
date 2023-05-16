using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MusicStoreMVCApp;
using MusicStoreMVCApp.Models;

namespace MusicStoreMVCApp.Controllers
{
    public class AccountController : Controller
    {
        MusicStoreAppEntities db = new MusicStoreAppEntities();

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Email);
            if (user.UserType == UserType.Seller)
            {
                ApprovalList sellerInApprovalList = db.ApprovalLists.Where(seller => seller.SellerEmail == model.Email).SingleOrDefault();
                // if no record of seller in approval list or
                // the status is other than 'Approved'
                if (sellerInApprovalList == null || sellerInApprovalList.Status != 1)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (user != null)
                    {
                        if (user.UserType == UserType.Admin)
                            return RedirectToAction("Index", "Admin");
                        else if (user.UserType == UserType.Customer)
                            return RedirectToAction("Index", "Customer");
                        else if (user.UserType == UserType.Seller)
                            return RedirectToAction("Index", "Seller");
                        return RedirectToAction("Error");
                    }
                    return RedirectToAction("Error");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, UserType = model.UserType };
                var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.UserType == UserType.Customer)
                    {
                        userManager.AddToRole(user.Id, "Customer");
                        await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        db.Customers.Add(new Customer()
                        {
                            Fname = model.Firstname,
                            Lname = model.Lastname,
                            Address = model.Address,
                            PhoneNumber = model.PhoneNumber,
                            UserId = Convert.ToInt32(user.Id)
                        });
                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                    else if (model.UserType == UserType.Seller)
                    {
                        userManager.AddToRole(user.Id, "Seller");

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");                     

                        ApprovalList approval = new ApprovalList() 
                        { 
                            SellerEmail = model.Email,
                            SellerFname = model.Firstname, 
                            SellerLname = model.Lastname, 
                            Address = model.Address, 
                            PhoneNumber = model.PhoneNumber 
                        };
                        db.ApprovalLists.Add(approval);
                        db.SaveChanges();
                        
                        return View(model);
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Error()
        {
            return View();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}