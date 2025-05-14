using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog7311_Part2.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please enter a product name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please select a category")]
        public ProductCategory Category { get; set; }
        
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Please enter a price")]
        [Range(0.01, 9999999.99, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Please enter a production date")]
        [Display(Name = "Production Date")]
        public DateTime ProductionDate { get; set; }
        
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        
        public int FarmerId { get; set; }
        
        [ForeignKey("FarmerId")]
        public Farmer? Farmer { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    
    public enum ProductCategory
    {
        Crop,
        Livestock,
        Dairy,
        Poultry,
        Fruit,
        Vegetable,
        GreenEnergy,
        Other
    }
} 