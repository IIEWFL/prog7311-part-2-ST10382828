using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog7311_Part2.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please enter a name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Department cannot be longer than 50 characters")]
        public string? Department { get; set; }
        
        [StringLength(20, ErrorMessage = "Employee number cannot be longer than 20 characters")]
        public string? EmployeeNumber { get; set; }
        
        [Phone]
        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters")]
        public string? PhoneNumber { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 