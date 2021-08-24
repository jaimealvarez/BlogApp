using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogApp.Models;

namespace BlogApp.Controllers.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly EntityContext _context;

        public PostController(EntityContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Posts.ToArrayAsync();
            var response = posts.Select(u => new {
                id = u.Id,
                pending = u.Pending,
                approved = u.Approved,
                submitDateTime = u.SubmitDateTime,
                approvedDatTime = u.ApprovedDateTime,
                content = u.Content
            });
            return Ok(response);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPendingPosts()
        {
            var posts = await _context.Posts.Where(p => p.Submitted && p.Pending).ToArrayAsync();
            var response = posts.Select(u => new {
                id = u.Id,
                pending = u.Pending,
                approved = u.Approved,
                submitDateTime = u.SubmitDateTime,
                content = u.Content
            });
            return Ok(response);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetApprovedPosts()
        {
            var posts = await _context.Posts.Where(p => p.Approved).ToArrayAsync();
            var response = posts.Select(u => new {
                id = u.Id,
                pending = u.Pending,
                approved = u.Approved,
                approvedDateTime = u.ApprovedDateTime,
                content = u.Content
            });
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            if (PostExists(id))
            {
                Post postdb = await _context.Posts.FindAsync(id);

                if (!post.Approved)
                    postdb.Submitted = false;
                else
                    postdb.ApprovedDateTime = DateTime.Now;

                postdb.Pending = false;
                postdb.Approved = post.Approved;

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

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            Post post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            post.Approved = false;
            post.Pending = true;
            post.SubmitDateTime = DateTime.Now;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }
    }
}
