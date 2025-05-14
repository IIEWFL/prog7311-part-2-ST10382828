using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Prog7311_Part2.Data
{
    public static class SeedProductImages
    {
        public static Task EnsureSeedProductImagesExist(IWebHostEnvironment env)
        {
            // Define the folder where seed images will be stored
            string seedImagesFolder = Path.Combine(env.WebRootPath, "images", "seed_products");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(seedImagesFolder))
            {
                Directory.CreateDirectory(seedImagesFolder);
                Console.WriteLine($"Created seed product images directory: {seedImagesFolder}");
            }
            
            // Just log a reminder for manual image placement
            Console.WriteLine($"Manual image placement: Please place product images manually in: {seedImagesFolder}");
            Console.WriteLine("The following images are expected:");
            foreach (var imagePath in GetSeedProductImagePaths())
            {
                Console.WriteLine($"  - {Path.GetFileName(imagePath)}");
            }
            
            Console.WriteLine("Seed product images setup completed.");
            
            return Task.CompletedTask;
        }
        
        public static string[] GetSeedProductImagePaths()
        {
            return new[]
            {
                "/images/seed_products/carrots.jpg",
                "/images/seed_products/lettuce.jpg",
                "/images/seed_products/apples.jpg",
                "/images/seed_products/strawberries.jpg",
                "/images/seed_products/solar_panel.jpg",
                "/images/seed_products/wind_turbine.jpg",
                "/images/seed_products/chickens.jpg",
                "/images/seed_products/yogurt.jpg",
                "/images/seed_products/cheese.jpg",
                "/images/seed_products/wheat.jpg"
            };
        }
    }
} 