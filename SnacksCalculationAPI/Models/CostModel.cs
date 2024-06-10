﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnacksCalculationAPI.Models
{
    [Table("CostInfo")]
    public class CostModel
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Date { get; set; }
        public double Amount { get; set; }
        public string Item { get; set; }
    }
}
