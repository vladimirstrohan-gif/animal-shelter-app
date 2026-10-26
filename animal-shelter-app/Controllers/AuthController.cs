using animal_shelter_app.Data;
using animal_shelter_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace animal_shelter_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest(new { message = "Пользователь с таким email уже существует!" });
            }

            model.PasswordHash = _passwordHasher.HashPassword(model, model.PasswordHash);

            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Регистрация прошла успешно!", userId = model.Id, model.Email, model.FullName, model.Role });
        }
    }
}
