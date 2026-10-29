using System.Collections.Generic;

namespace animal_shelter_app.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;         // Заголовок статьи
        public string? Category { get; set; }                     // Категория (напр. "Поради")
        public string Description { get; set; } = string.Empty;   // Короткий опис для прев'ю
        public string Content { get; set; } = string.Empty;       // Повний текст статті
        public string? ImageUrl { get; set; }                     // Посилання на фото
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата створення
    }
}