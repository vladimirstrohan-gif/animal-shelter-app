using animal_shelter_app.Data;
using animal_shelter_app.DTOs;
using animal_shelter_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace animal_shelter_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: Отримати список усіх статей (доступно всім) -> /api/articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            return await _context.Articles
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        // 2. GET: Отримати одну статтю за ID -> /api/articles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound(new { message = "Статтю з таким ID не знайдено." });
            }

            return article;
        }

        // 3. POST: Додати нову статтю (тільки для Admin) -> /api/articles
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Article>> CreateArticle([FromBody] ArticleCreateDto dto)
        {
            var article = new Article
            {
                Title = dto.Title,
                Category = dto.Category,
                Description = dto.Description,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        // 4. PUT: Оновити статтю (тільки для Admin) -> /api/articles/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] ArticleCreateDto dto)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound(new { message = "Статтю не знайдено." });
            }

            article.Title = dto.Title;
            article.Category = dto.Category;
            article.Description = dto.Description;
            article.Content = dto.Content;
            article.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Статтю успішно оновлено!", article });
        }

        // 5. DELETE: Видалити статтю (тільки для Admin) -> /api/articles/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound(new { message = "Статтю не знайдено." });
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Статтю видалено з бази." });
        }
    }
}