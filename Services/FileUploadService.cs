
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prog7311_Part2.Services
{
    // MVC Pattern Implementation:
    // This service class demonstrates the separation of concerns part of the MVC pattern
    // mentioned in the report. It handles file processing logic separately from
    // controllers, views, and models, keeping the code organized and maintainable.
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// Uploads a product image and returns the relative URL path
        /// Implementation based on Microsoft's recommended file upload patterns
      
        /// Adapted from: Microsoft (2024) 'File uploads in ASP.NET Core', Microsoft Docs.27 September 2024.[online]
        /// Available at: https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
        /// (Accessed: 13 May 2025).
        public async Task<string> UploadProductImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Validate file type
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!Array.Exists(allowedExtensions, x => x == fileExtension))
            {
                throw new InvalidOperationException("Invalid file type. Only image files (jpg, jpeg, png, gif) are allowed.");
            }

            // Create unique filename
            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            
            // Make sure the uploads directory exists
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);
            
            // Save the file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return the relative path to the file
            return $"/uploads/products/{uniqueFileName}";
        }
    }
} 