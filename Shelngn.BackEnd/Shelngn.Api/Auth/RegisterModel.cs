using System.ComponentModel.DataAnnotations;

namespace Shelngn.Api.Auth
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; } = null!;

        public string? UserName { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
