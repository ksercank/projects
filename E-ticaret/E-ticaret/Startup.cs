using E_ticaret.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(E_ticaret.Startup))]
namespace E_ticaret
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUser();
        }
        private void createRolesandUser()
        {
            ApplicationDbContext ctx = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "sercankilic7052@hotmail.com";
                user.Email = "sercankilic7052@hotmail.com";
                string sifre = "qwe123qweSk+-";
                var kullanici = userManager.Create(user, sifre);

                if (kullanici.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("Uye"))
            {
                var role = new IdentityRole();
                role.Name = "Uye";
                roleManager.Create(role);
            }
        }
    }
}
