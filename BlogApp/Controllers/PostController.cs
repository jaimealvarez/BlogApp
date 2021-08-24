using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class PostController : Controller
    {
        private readonly EntityContext _context;

        public PostController(EntityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create([Bind("Content")] Post post)
        {
            try
            {
                if (ModelState.IsValid && post.Content != null && post.Content.Length > 0)
                {
                    post.Submitted = false;
                    post.Pending = false;
                    post.Approved = false;
                    post.SubmitDateTime = DateTime.Now;

                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(IndexPendingAndRejected));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View();
        }

        public async Task<IActionResult> IndexPending()
        {
            var posts = await _context.Posts.Where(p => (p.Submitted && (p.Pending || p.Approved))).OrderBy(p => p.SubmitDateTime).ToArrayAsync();
            var response = posts.Select(u => new {
                id = u.Id,
                submitDateTime = u.SubmitDateTime,
                content = u.Content
            });
            return View(posts);
        }

        public async Task<IActionResult> IndexPendingAndRejected()
        {
            var posts = await _context.Posts.ToArrayAsync();
            var response = posts.Select(u => new {
                id = u.Id,
                submitted = u.Submitted,
                pending = u.Pending,
                approved = u.Approved,
                content = u.Content
            });
            return View(posts);
        }

        public async Task<IActionResult> Approve(int id)
        {
            if (PostExists(id))
            {
                Post postdb = await _context.Posts.FindAsync(id);

                postdb.Pending = false;
                postdb.Approved = true;
                postdb.ApprovedDateTime = DateTime.Now;

                _context.Entry(postdb).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(IndexPending));
        }

        public async Task<IActionResult> Reject(int id)
        {
            if (PostExists(id))
            {
                Post postdb = await _context.Posts.FindAsync(id);

                postdb.Submitted = false;
                postdb.Pending = false;
                postdb.Approved = false;

                _context.Entry(postdb).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(IndexPending));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return RedirectToAction(nameof(IndexPending));
            }

            try
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexPending));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
                throw;
            }
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var postToUpdate = await _context.Posts.FirstOrDefaultAsync(s => s.Id == id);
            postToUpdate.Submitted = false;
            postToUpdate.Pending = false;
            postToUpdate.Approved = false;
            if (await TryUpdateModelAsync<Post>(
                postToUpdate,
                "",
                s => s.Content, s => s.SubmitDateTime, s => s.Pending, s => s.Approved))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(IndexPendingAndRejected));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            return View(postToUpdate);
        }

        public async Task<IActionResult> Submit(int id)
        {
            if (PostExists(id))
            {
                Post postdb = await _context.Posts.FindAsync(id);

                postdb.Submitted = true;
                postdb.Pending = true;
                postdb.Approved = false;
                postdb.SubmitDateTime = DateTime.Now;

                _context.Entry(postdb).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(IndexPendingAndRejected));
        }

    }
}
