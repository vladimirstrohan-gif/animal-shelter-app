namespace animal_shelter_app.Models
{
    public enum AnimalStatus
    {
        LookingForHome, // Ищет хозяина
        FoundHome,      // Нашел хозяина
        InTreatment,    // На лечении / В клинике
        InShelter       // Просто в приюте (на карантине или адаптации)    
    }
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty; 
        public string? Breed { get; set; }                  
        public int Age { get; set; }                        
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }               // Ссылка на фото питомца
        public string City { get; set; } = string.Empty;

        public AnimalStatus Status { get; set; } = AnimalStatus.InShelter;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
