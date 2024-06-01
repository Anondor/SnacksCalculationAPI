using System.ComponentModel.DataAnnotations;

namespace SnacksCalculationAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
