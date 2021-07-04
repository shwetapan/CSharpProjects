using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BloggerBlog.Data;
using BloggerBlog.Models;
using Microsoft.AspNetCore.Authorization;
using BloggerBlog.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BloggerBlog.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var post = _context.Post.Include(x => x.Author);
            int pageSize = 3;
            return View(await PaginatedList<Post>.CreateAsync(post.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Posts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.Include(z=>z.Author).Include(a=>a.PostTags).ThenInclude(b=>b.Tags).Include(x=>x.PostCategories).ThenInclude(y=>y.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            var CategoryList = _context.Category.ToList();
            var TagList = _context.Tags.ToList();
            CreatePostViewModel vm = new CreatePostViewModel();
            vm.Categories = CategoryList.Select(x => new SelectListItem()
            {
                Text = x.Title,
                Value = x.Id.ToString()
            }).ToList();

            vm.PostTags = TagList.Select(x => new SelectListItem()
            {
                Text = x.Title,
                Value = x.Id.ToString()
            }).ToList();

            return View(vm);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel vm)
        {
            var selectedCategory = vm.Categories.Where(x => x.Selected).Select(x=>x.Value).Select(int.Parse).ToList();
            var selectedTags = vm.PostTags.Where(x => x.Selected).Select(x=>x.Value).Select(int.Parse).ToList();
            if (ModelState.IsValid)
            {
                var post = new Post
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    PublishedDate = vm.PublishedDate,
                    ApplicationUserId = _userManager.GetUserId(HttpContext.User)
                };
                foreach(var item in selectedCategory)
                {
                    post.PostCategories.Add(new PostCategory()
                    {
                        Post = post,
                        CategoryId = item
                    });
                }

                foreach (var item in selectedTags)
                {
                    post.PostTags.Add(new PostTags()
                    {
                        Post = post,
                        TagsId = item
                    });
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,PublishedDate,ApplicationUserId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
