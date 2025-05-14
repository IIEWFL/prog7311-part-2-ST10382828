using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;

namespace Prog7311_Part2.Data
{
    /// <summary>
    /// Database initialization and seeding class
    /// </summary>
 
    /// Seeding approach adapted from: Microsoft (2025) 'Data Seeding',
    /// Microsoft Docs. 15 January 2025. Available at: https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
    /// (Accessed: 13 May 2025).
 
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Create the database if it doesn't exist and apply migrations
            context.Database.Migrate();

            // Check if we already have users
            if (context.Users.Any())
            {
                return;  // DB has been seeded
            }

            // Define image paths for products
            var productImageMap = new Dictionary<string, string>
            {
                { "Organic Carrots", "/images/seed_products/carrots.jpg" },
                { "Fresh Lettuce", "/images/seed_products/lettuce.jpg" },
                { "Organic Apples", "/images/seed_products/apples.jpg" },
                { "Fresh Strawberries", "/images/seed_products/strawberries.jpg" },
                { "Solar Panel System", "/images/seed_products/solar_panel.jpg" },
                { "Wind Turbine Kit", "/images/seed_products/wind_turbine.jpg" },
                { "Free-Range Chickens", "/images/seed_products/chickens.jpg" },
                { "Homemade Yogurt", "/images/seed_products/yogurt.jpg" },
                { "Artisanal Cheese", "/images/seed_products/cheese.jpg" },
                { "Organic Wheat", "/images/seed_products/wheat.jpg" }
            };

            // Seed employee user
            var employeeUser = new User
            {
                Username = "admin",
                Email = "admin@agrienergy.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = UserRole.Employee,
                CreatedAt = DateTime.Now
            };

            context.Users.Add(employeeUser);
            context.SaveChanges();

            // Seed employee profile
            var employee = new Employee
            {
                Name = "Admin User",
                Department = "Management",
                EmployeeNumber = "EMP001",
                PhoneNumber = "011-555-0000",
                UserId = employeeUser.Id,
                CreatedAt = DateTime.Now
            };
            
            context.Employees.Add(employee);
            context.SaveChanges();

            // Seed farmer user
            var farmerUser = new User
            {
                Username = "farmer1",
                Email = "farmer1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("farmer123"),
                Role = UserRole.Farmer,
                CreatedAt = DateTime.Now
            };

            context.Users.Add(farmerUser);
            context.SaveChanges();

            // Seed farmer profile
            var farmer = new Farmer
            {
                Name = "Green Fields Farm",
                Location = "Cape Town, South Africa",
                Description = "Sustainable farm specializing in organic vegetables and solar energy integration.",
                PhoneNumber = "021-555-1234",
                UserId = farmerUser.Id,
                CreatedAt = DateTime.Now
            };

            context.Farmers.Add(farmer);
            context.SaveChanges();

            // Seed products using local image files
            var products = new List<Product>
            {
                // Vegetables
                new Product {
                    Name = "Organic Carrots",
                    Category = ProductCategory.Vegetable,
                    Description = "Fresh organic carrots grown using sustainable farming practices.",
                    Price = 15.99M,
                    ProductionDate = DateTime.Now.AddDays(-3),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Organic Carrots"]
                },
                new Product {
                    Name = "Fresh Lettuce",
                    Category = ProductCategory.Vegetable,
                    Description = "Crisp, fresh lettuce leaves harvested daily.",
                    Price = 18.50M,
                    ProductionDate = DateTime.Now.AddDays(-1),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Fresh Lettuce"]
                },
                
                // Fruits
                new Product {
                    Name = "Fresh Strawberries",
                    Category = ProductCategory.Fruit,
                    Description = "Sweet and juicy strawberries grown in our greenhouse.",
                    Price = 35.75M,
                    ProductionDate = DateTime.Now.AddDays(-2),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Fresh Strawberries"]
                },
                new Product {
                    Name = "Organic Apples",
                    Category = ProductCategory.Fruit,
                    Description = "Crisp and delicious apples free from pesticides.",
                    Price = 22.99M,
                    ProductionDate = DateTime.Now.AddDays(-10),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Organic Apples"]
                },
                
                // Green Energy
                new Product {
                    Name = "Solar Panel System",
                    Category = ProductCategory.GreenEnergy,
                    Description = "Energy-efficient solar panel system for agricultural use.",
                    Price = 5750.00M,
                    ProductionDate = DateTime.Now.AddDays(-45),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Solar Panel System"]
                },
                new Product {
                    Name = "Wind Turbine Kit",
                    Category = ProductCategory.GreenEnergy,
                    Description = "Small-scale wind turbine kit suitable for farms and rural properties.",
                    Price = 8999.99M,
                    ProductionDate = DateTime.Now.AddDays(-60),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Wind Turbine Kit"]
                },
                
                // Livestock
                new Product {
                    Name = "Free-Range Chickens",
                    Category = ProductCategory.Livestock,
                    Description = "Ethically raised free-range chickens fed on organic feed.",
                    Price = 85.00M,
                    ProductionDate = DateTime.Now.AddDays(-7),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Free-Range Chickens"]
                },
                
                // Dairy
                new Product {
                    Name = "Homemade Yogurt",
                    Category = ProductCategory.Dairy,
                    Description = "Creamy homemade yogurt made from our farm's own milk.",
                    Price = 27.50M,
                    ProductionDate = DateTime.Now.AddDays(-1),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Homemade Yogurt"]
                },
                new Product {
                    Name = "Artisanal Cheese",
                    Category = ProductCategory.Dairy,
                    Description = "Handcrafted cheese made from our farm's own milk.",
                    Price = 65.00M,
                    ProductionDate = DateTime.Now.AddDays(-14),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Artisanal Cheese"]
                },
                
                // Crops
                new Product {
                    Name = "Organic Wheat",
                    Category = ProductCategory.Crop,
                    Description = "Locally grown organic wheat for baking and cooking.",
                    Price = 42.99M,
                    ProductionDate = DateTime.Now.AddDays(-30),
                    FarmerId = farmer.Id,
                    CreatedAt = DateTime.Now,
                    ImageUrl = productImageMap["Organic Wheat"]
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
} 