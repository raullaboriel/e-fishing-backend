using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efishingAPI.Models
{
    [Table("CartProducts")]
    public class CartProduct
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Users(id)")]
        public int id_user { get; set; }
        [ForeignKey("Products(id)")]
        public int id_product { get; set; }
        [Required]
        public int amount { get; set; }
    }
}
