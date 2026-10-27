using System.ComponentModel.DataAnnotations;

namespace animal_shelter_app.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный формат email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен.")]
        public string Password { get; set; } = string.Empty;
    }
}
