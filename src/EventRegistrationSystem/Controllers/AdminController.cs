﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dapper;
using EventRegistrationSystem.Attributes;
using EventRegistrationSystem.Authorization;
using EventRegistrationSystem.Data;
using EventRegistrationSystem.Identity;
using EventRegistrationSystem.Models;
using EventRegistrationSystem.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace EventRegistrationSystem.Controllers
{
    [Authorize]
    [AuthorizeRoles(Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private ApplicationUserManager _userManager;

        public AdminController(IRoleRepository roleRepository, ApplicationUserManager userManager = null)
        {
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        protected ApplicationUserManager UserManager => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        // GET: Admin/Users
        public ActionResult Users()
        {
            //var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            //var users = userStore.Users.ToList();
            //return View(users);

            // Get all users directly from the database
            List<ApplicationUser> users = new List<ApplicationUser>();

            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                users = connection.Query<ApplicationUser>(@"
            SELECT Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                   PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                   LockoutEndDateUtc, LockoutEnabled, AccessFailedCount
            FROM AspNetUsers
            ORDER BY UserName").ToList();

            }

            return View(users);
        }

        // GET: Admin/UserRoles/userId
        public ActionResult UserRoles(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;

            var userRoles = _roleRepository.GetUserRoles(id);
            ViewBag.IsAdmin = userRoles.Any(r => r.RoleName == Roles.Admin);
            ViewBag.IsOrganizer = userRoles.Any(r => r.RoleName == Roles.Organizer);
            ViewBag.IsUser = userRoles.Any(r => r.RoleName == Roles.User);

            return View();
        }

        // POST: Admin/AddToRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToRole(string userId, string roleName)
        {
            var user = UserManager.FindById(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            _roleRepository.AddUserToRole(userId, roleName);

            TempData["SuccessMessage"] = $"Added {user.UserName} to the {roleName} role.";
            return RedirectToAction("UserRoles", new { id = userId });
        }

        // POST: Admin/RemoveFromRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromRole(string userId, string roleName)
        {
            var user = UserManager.FindById(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Don't allow removing the last admin
            if (roleName == Roles.Admin)
            {
                var adminUsers = UserManager.Users
                    .Where(u => _roleRepository.UserIsInRole(u.Id, Roles.Admin))
                    .Count();

                if (adminUsers <= 1)
                {
                    TempData["ErrorMessage"] = "Cannot remove the last administrator.";
                    return RedirectToAction("UserRoles", new { id = userId });
                }
            }

            _roleRepository.RemoveUserFromRole(userId, roleName);

            TempData["SuccessMessage"] = $"Removed {user.UserName} from the {roleName} role.";
            return RedirectToAction("UserRoles", new { id = userId });
        }
    }
}
