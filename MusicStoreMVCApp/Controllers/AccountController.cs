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

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await userManager.FindByNameAsync(model.Email);
                    if (user != null)
                    {
                        if (user.UserType == UserType.Admin)
                            return RedirectToAction("Index", "Admin");
                        else if (user.UserType == UserType.Customer)
                            return RedirectToAction("Index", "Customer");
                        else if (user.UserType == UserType.Seller)
                            return RedirectToAction("Index", "Seller");
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
                if(model.UserType == UserType.Customer)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, UserType = UserType.Customer };
                    var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(user.Id, "Customer");
                        await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        return RedirectToAction("Index", "Home");
                    }
                    AddErrors(result);
                }
                else if(model.UserType == UserType.Seller)
                {
                    ApprovalList approval = new ApprovalList() { SellerFname = model.Firstname, SellerLname = model.Lastname, Address = model.Address, PhoneNumber = model.PhoneNumber };
                    db.ApprovalLists.Add(approval);
                    db.SaveChanges();
                }
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