using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efishingAPI.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(50)]
        public string name { get; set; }
        [Required]
        [StringLength(40)]
        public string brand { get; set; }
        [Required]
        [Column(TypeName = "decimal(15, 2)")]
        public decimal price { get; set; }
        [Required]
        [StringLength(30)]
        public string model { get; set; }
        [Required]
        [StringLength(2000)]
        public string description { get; set; }
        [Required]
        [StringLength(30)]
        public string category { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal size { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal weight { get; set; }
        [Required]
        public int stock { get; set; }

    }
}
