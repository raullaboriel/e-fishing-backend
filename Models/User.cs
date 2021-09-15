using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace efishingAPI.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int id { get; set; }
        [StringLength(50, MinimumLength = 2)][Required]
        public string name { get; set; }
        [StringLength(50, MinimumLength = 2)][Required]
        public string lastname { get; set; }
        [StringLength(50, MinimumLength = 5)][Required]
        public string email { get; set; }
        [StringLength(100, MinimumLength = 8)][Required]
        public string password { get; set; }
        [Required]
        public bool admin { get; set; }

    }
}
