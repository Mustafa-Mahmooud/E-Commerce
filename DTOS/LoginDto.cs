using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOS
{
    public class LoginDto
    {

        [Required]
        public string DispalyName { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
