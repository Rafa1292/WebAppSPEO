using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public UserView GetUserView(string userId)
        {
            UserView userView = new UserView();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == userId);

            if (user == null)
            {
                userView = null;
                return userView;
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roles = roleManager.Roles.ToList();
            var rolesView = new List<RoleView>();

            if (user.Roles != null)
            {
                foreach (var item in user.Roles)
                {
                    var role = roles.Find(r => r.Id == item.RoleId);
                    var roleView = new RoleView
                    {
                        RoleID = role.Id,
                        Name = role.Name
                    };
                    rolesView.Add(roleView);
                }
            }

            userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserID = user.Id,
                Roles = rolesView
            };
            return userView;
        }

        public void RolesViewBag()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roleList = roleManager.Roles.ToList();
            roleList.Add(new IdentityRole { Id = "", Name = "Seleccione un rol" });
            roleList = roleList.OrderBy(r => r.Id).ToList();
            ViewBag.RoleID = new SelectList(roleList, "Id", "Name");
        }
        // GET: Users
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            var usersView = new List<UserView>();
            foreach (var user in users)
            {
                if (user.UserName != "rvilla3452@gmail.com")
                {
                    UserView userView = new UserView
                    {
                        Email = user.Email,
                        Name = user.UserName,
                        UserID = user.Id
                    };
                    usersView.Add(userView);
                }
            }
            return View(usersView);
        }

        public ActionResult Roles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserView userView = GetUserView(userId);

            if (userView == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RolesViewBag();

            return View(userView);
        }

        [HttpGet]
        public ActionResult AddRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserView userView = GetUserView(userId);

            if (userView == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolesViewBag();
            return View(userView);
        }

        [HttpPost]
        public ActionResult AddRole(string userId, FormCollection form)
        {
            RolesViewBag();
            var roleId = Request["RoleId"];
            UserView userView = GetUserView(userId);

            if (userView == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(roleId))
            {
                ViewBag.Error = "Debes seleccionar un rol";
                return View(userView);
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roleToAdd = roleManager.Roles.ToList().Find(r => r.Id == roleId);

            if (!userManager.IsInRole(userView.UserID, roleToAdd.Name))
            {
                userManager.AddToRole(userId, roleToAdd.Name);
            }

            userView = GetUserView(userId);
            return View("Roles", userView);
        }

        public ActionResult Delete(string userId, string roleId)
        {
            if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var user = userManager.Users.ToList().Find(u => u.Id == userId);
            var role = roleManager.Roles.ToList().Find(r => r.Id == roleId);

            if (userManager.IsInRole(userId, role.Name))
            {
                userManager.RemoveFromRole(userId, role.Name);
            }

            UserView userView = GetUserView(userId);
            return View("Roles", userView);
        }

        public ActionResult DeleteUser(string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var deleteUser = userManager.Users.ToList().Find(u => u.Id == userId);
            userManager.Delete(deleteUser);

            var users = userManager.Users.ToList();
            var usersView = new List<UserView>();
            foreach (var user in users)
            {
                if (user.UserName != "rvilla3452@gmail.com")
                {
                    UserView userView = new UserView
                    {
                        Email = user.Email,
                        Name = user.UserName,
                        UserID = user.Id
                    };
                    usersView.Add(userView);
                }
            }

            return View("index", usersView);

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}