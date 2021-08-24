using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly EntityContext _context;

        public LoginController(EntityContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin([Bind] User user)
        {
            User userdb = _context.Users.Where(u => u.UserName.Equals(user.UserName) && u.Password.Equals(user.Password)).FirstOrDefault();

            //var users = new User();
            //var allUsers = users.GetUsers().FirstOrDefault();
            //if (users.GetUsers().Any(u => u.UserName == user.UserName))
            if (userdb != null)
            {
                var userClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userdb.UserName),
                    new Claim(ClaimTypes.GivenName, userdb.Name),
                    new Claim(ClaimTypes.Email, userdb.EmailId)
                };

                var userIdentity = new ClaimsIdentity(userClaims, "User Identity");

                var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });
                HttpContext.SignInAsync(userPrincipal);

                string role = userdb.Role;

                if (role.Equals("writer"))
                {
                    return RedirectToAction("IndexPendingAndRejected", "Post"); 
                    //View("~/Views/Post/IndexPendingAndRejected.cshtml");
                }
                else
                {
                    return RedirectToAction("IndexPending", "Post");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
