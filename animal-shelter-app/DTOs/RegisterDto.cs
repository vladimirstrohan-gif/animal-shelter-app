using animal_shelter_app.Models;
using System.ComponentModel.DataAnnotations;

namespace animal_shelter_app.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный формат email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен.")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "ФИО обязательно.")]
        public string FullName { get; set; } = string.Empty;

    }
}
