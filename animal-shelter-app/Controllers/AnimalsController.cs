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

        // 3. POST: Добавить нового питомца  -> /api/animals
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Animal>> CreateAnimal([FromBody] AnimalCreateDto dto)
        {
            var animal = new Animal
            {
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                Age = dto.Age,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                City = dto.City,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        // 4. PUT: Обновить данные питомца  -> /api/animals/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAnimal(int id, [FromBody] AnimalCreateDto dto)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound(new { message = "Питомец не найден." });
            }

            animal.Name = dto.Name;
            animal.Species = dto.Species;
            animal.Breed = dto.Breed;
            animal.Age = dto.Age;
            animal.Description = dto.Description;
            animal.Status = dto.Status;
            animal.ImageUrl = dto.ImageUrl;
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
