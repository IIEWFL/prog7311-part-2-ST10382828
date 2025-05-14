using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog7311_Part2.Models
{
    public class Farmer
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Location { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        public ICollection<Product> Products { get; set; } = new List<Product>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 