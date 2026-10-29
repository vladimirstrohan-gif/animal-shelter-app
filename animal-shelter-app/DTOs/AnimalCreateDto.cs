using animal_shelter_app.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace animal_shelter_app.DTOs
{
    public class AnimalCreateDto
    {
        [Required(ErrorMessage = "Кличка питомца обязательна.")]
        [StringLength(30, ErrorMessage = "Имя не должно превышать 30 символов.")]
        [MinLength(3, ErrorMessage = "Минимум 3 символа.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вид животного обязателен.")]
        public string Species { get; set; } = string.Empty;

        public string? Breed { get; set; }

        [Range(0, 30, ErrorMessage = "Возраст должен быть в диапазоне от 0 до 30 лет.")]
        public int Age { get; set; }

        [StringLength(500, ErrorMessage = "Описание слишком длинное (максимум 500 символов).")]
        public string Description { get; set; } = string.Empty;

        // Поле для загрузки файла картинки с фронтенда
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Город обязателен.")]
        public string City { get; set; } = string.Empty;

        public AnimalStatus Status { get; set; } = AnimalStatus.LookingForHome;
    }
}