using animal_shelter_app.Data;
using animal_shelter_app.Models;
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

        // 1. GET: Получить список всех животных -> /api/animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            return await _context.Animals.ToListAsync();
        }

        // 2. GET: Получить одно животное по ID -> /api/animals/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _context.Animals.FindAsync(id);

            if (animal == null)
            {
                return NotFound(new { message = "Питомец с таким ID не найден." });
            }

            return animal;
        }

        // 3. POST: Добавить нового питомца -> /api/animals
        [HttpPost]
        public async Task<ActionResult<Animal>> CreateAnimal([FromBody] Animal animal)
        {
            // Сбрасываем Id, чтобы база данных сгенерировала его автоматически
            animal.Id = 0;
            animal.CreatedAt = DateTime.UtcNow;

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        // 4. PUT: Обновить данные питомца -> /api/animals/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound(new { message = "Питомец не найден." });
            }

            // Обновляем поля
            animal.Name = updatedAnimal.Name;
            animal.Species = updatedAnimal.Species;
            animal.Breed = updatedAnimal.Breed;
            animal.Age = updatedAnimal.Age;
            animal.Description = updatedAnimal.Description;
            animal.Status = updatedAnimal.Status;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Данные питомца успешно обновлены!", animal });
        }

        // 5. DELETE: Удалить питомца -> /api/animals/{id}
        [HttpDelete("{id}")]
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
