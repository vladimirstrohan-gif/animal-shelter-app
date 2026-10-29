namespace animal_shelter_app.DTOs
{
    public class ArticleCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}