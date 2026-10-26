namespace animal_shelter_app.Models
{

    public enum UserRole
    {
        Volunteer,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Volunteer;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
