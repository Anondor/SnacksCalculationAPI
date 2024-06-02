using System.ComponentModel.DataAnnotations.Schema;

namespace SnacksCalculationAPI.Models
{
    [Table("UserCostInfo")]
    public class UserInformationModel
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public string Date { get; set; }
        public double Amount {  get; set; }
        public string ItemName  { get; set; }
    }
}
