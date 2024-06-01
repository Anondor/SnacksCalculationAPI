using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnacksCalculationAPI.Models
{
    [Table("UserInfo")]
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int UserType{ get; set; }
        public string Password { get; set; }
    }
}
