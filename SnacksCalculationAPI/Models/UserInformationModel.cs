using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnacksCalculationAPI.Models
{
    [Table("AccountTable")]
    public class UserInformationModel
    {
        [Key]
        public int Id { get; set; }
        public int UserId {  get; set; }
        public string Date { get; set; }
        public double Amount {  get; set; }
    }
}
