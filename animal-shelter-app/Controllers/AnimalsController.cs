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
    public class AnimalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimalsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: Получить список всех животных (доступно всем) -> /api/animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            return await _context.Animals.ToListAsync();
        }

        // 2. GET: Получить одно животное по ID  -> /api/animals/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _context.Animals.FindAsync(id);

            if (animal == null)
            {
                return NotFound(new { message = "Питомец с таким ID не найден." });
            }

            return animal;
        }

        // 3. POST: Добавить нового питомца (с поддержкой загрузки файла) -> /api/animals
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Animal>> CreateAnimal([FromForm] AnimalCreateDto dto)
        {
            string imageUrl = "https://example.com/images/default.jpg"; // Фолбек, если файл не передан

            // Если файл передан, сохраняем его на сервер в wwwroot/images
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                // Формируем корректный URL с учетом текущего хоста/домена
                var host = Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? Request.Host.Value;
                var scheme = Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? Request.Scheme;
                imageUrl = $"{scheme}://{host}/images/{uniqueFileName}";
            }

            var animal = new Animal
            {
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                Age = dto.Age,
                Description = dto.Description,
                ImageUrl = imageUrl, // Сохраняем готовый URL картинки
                City = dto.City,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        // 4. PUT: Обновить данные питомца (с возможностью заменить фото) -> /api/animals/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAnimal(int id, [FromForm] AnimalCreateDto dto)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound(new { message = "Питомец не найден." });
            }

            // Если загружен новый файл картинки, сохраняем его и обновляем URL
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                var host = Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? Request.Host.Value;
                var scheme = Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? Request.Scheme;
                animal.ImageUrl = $"{scheme}://{host}/images/{uniqueFileName}";
            }

            animal.Name = dto.Name;
            animal.Species = dto.Species;
            animal.Breed = dto.Breed;
            animal.Age = dto.Age;
            animal.Description = dto.Description;
            animal.Status = dto.Status;
            animal.City = dto.City;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Данные питомца успешно обновлены!", animal });
        }

        // 5. DELETE: Удалить питомца -> /api/animals/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound(new { message = "Питомец не найден." });
            }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Питомец удален из базы." });
        }
    }
}