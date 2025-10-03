using AdvancedRoutingDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Docs.Samples;

namespace AdvancedRoutingDemo.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // Route with multiple constraints
        [Route("blog/{year:int:min(2000)}/{month:int:range(1,12)}/{title}")]
        public async Task<IActionResult> Details(int year, int month, string title)
        {
            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(b =>
                    b.PublishedDate.Year == year &&
                    b.PublishedDate.Month == month &&
                    b.Title.ToLower().Replace(" ", "-") == title.ToLower());

            if (post == null)
            {
                return NotFound();
            }

            // استفاده صحیح از MyDisplayRouteInfo - فقط برای نمایش اطلاعات route
            // این خط فقط برای تست است، می‌توانید حذفش کنید
            // return ControllerContext.MyDisplayRouteInfo(msg: $"Year: {year}, Month: {month}, Title: {title}");

            ViewBag.RouteInfo = ControllerContext.ToCtxString(msg: $"Year: {year}, Month: {month}, Title: {title}");
            return View(post);
        }

        // Optional parameters with query string
        [Route("blog")]
        public async Task<IActionResult> Index(string category = null, string author = null, int page = 1)
        {
            var posts = await _context.BlogPosts
                .Where(b =>
                    (category == null || b.Title.Contains(category)) &&
                    (author == null || b.Author == author))
                .OrderByDescending(b => b.PublishedDate)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToListAsync();

            ViewBag.Category = category;
            ViewBag.Author = author;
            ViewBag.Page = page;

            ViewBag.RouteInfo = ControllerContext.ToCtxString(msg: $"Category: {category}, Author: {author}, Page: {page}");
            return View(posts);
        }

        // Route with regex constraint
        [HttpGet("blog/archive/{year:regex(^\\d{{4}}$)}")]
        public async Task<IActionResult> Archive(int year)
        {
            var posts = await _context.BlogPosts
                .Where(b => b.PublishedDate.Year == year)
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();

            ViewBag.Year = year;

            ViewBag.RouteInfo = ControllerContext.ToCtxString(msg: $"Year: {year}");
            return View(posts);
        }

        // مثال اضافه برای نمایش کارایی بیشتر
        [Route("blog/tag/{tagName}")]
        public async Task<IActionResult> ByTag(string tagName, int page = 1)
        {
            var posts = await _context.BlogPosts
                .Where(b => b.Content.Contains(tagName) || b.Title.Contains(tagName))
                .OrderByDescending(b => b.PublishedDate)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToListAsync();

            ViewBag.TagName = tagName;
            ViewBag.Page = page;

            ViewBag.RouteInfo = ControllerContext.ToCtxString(msg: $"Tag: {tagName}, Page: {page}");
            return View(posts);
        }
    }
}