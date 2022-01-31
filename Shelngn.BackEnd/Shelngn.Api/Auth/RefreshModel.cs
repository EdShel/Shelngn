using System.ComponentModel.DataAnnotations;

namespace Shelngn.Api.Auth
{
    public class RefreshModel
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
