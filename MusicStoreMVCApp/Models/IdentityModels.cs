using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace MusicStoreMVCApp.Models
{
    public enum UserType
    {
        Admin = 1,
        Customer = 2,
        Seller = 3
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public UserType UserType { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationUserClaim : IdentityUserClaim<long> { }
    public class ApplicationUserLogin : IdentityUserLogin<long> { }
    public class ApplicationUserRole : IdentityUserRole<long> { }
    public class ApplicationRole : IdentityRole<long, ApplicationUserRole> { }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDBInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class ApplicationDBInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            ConfigureDefaultRolesAndUsers();
            base.Seed(context);
        }

        private void ConfigureDefaultRolesAndUsers()
        {
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            if (!roleManager.RoleExists("Admin")) roleManager.Create(new ApplicationRole() { Name = "Admin" });
            if (!roleManager.RoleExists("Customer")) roleManager.Create(new ApplicationRole() { Name = "Customer" });
            if (!roleManager.RoleExists("Seller")) roleManager.Create(new ApplicationRole() { Name = "Seller" });
        }
    }
}