using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly EntityContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(EntityContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts.Where(p => !p.Pending && p.Approved).OrderByDescending(p => p.ApprovedDateTime).ToArrayAsync();
            var response = posts.Select(u => new {
                id = u.Id,
                pending = u.Pending,
                approved = u.Approved,
                submitDateTime = u.SubmitDateTime,
                content = u.Content
            });

            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public ActionResult Users()
        {
            return View(_context.Users);
        }
    }
}
