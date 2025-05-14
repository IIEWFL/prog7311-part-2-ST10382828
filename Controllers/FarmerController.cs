using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Prog7311_Part2.Services;
using Microsoft.AspNetCore.Http;

namespace Prog7311_Part2.Controllers
{
    [Authorize(Policy = "FarmerOnly")]
    public class FarmerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FarmerController> _logger;
        private readonly FileUploadService _fileUploadService;

        public FarmerController(AppDbContext context, ILogger<FarmerController> logger, FileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(farmer);
        }

        public IActionResult AddProduct()
        {
            _logger.LogInformation("AddProduct GET action called");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product, IFormFile productImage)
        {
            _logger.LogInformation("AddProduct POST action called with product: {@Product}", product);
            
            // If no file is uploaded but an ImageUrl is provided, 
            // remove the productImage error from ModelState
            // Adapted from: Microsoft (2024) 'Model Binding in ASP.NET Core', 
            // Microsoft Docs. 27 September 2024. Available at: https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding
            // (Accessed: 13 May 2025).
            if ((productImage == null || productImage.Length == 0) && !string.IsNullOrEmpty(product.ImageUrl))
            {
                // Remove validation errors for productImage since we have a URL
                ModelState.Remove("productImage");
            }
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid: {@ModelState}", ModelState);
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        _logger.LogWarning("Error for {Key}: {ErrorMessage}", entry.Key, error.ErrorMessage);
                    }
                }
                return View(product);
            }

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User ID claim is null or empty");
                    return RedirectToAction("Login", "Account");
                }

                var userId = int.Parse(userIdClaim);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    _logger.LogWarning("Farmer not found for user ID: {UserId}", userId);
                    return RedirectToAction("Login", "Account");
                }

                _logger.LogInformation("Found farmer: {FarmerId} for user: {UserId}", farmer.Id, userId);
                
                // Handle file upload if a file was submitted
                if (productImage != null && productImage.Length > 0)
                {
                    try
                    {
                        string imagePath = await _fileUploadService.UploadProductImageAsync(productImage);
                        product.ImageUrl = imagePath;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading product image");
                        ModelState.AddModelError("productImage", "Error uploading image. Make sure it's a valid image file.");
                        return View(product);
                    }
                }
                // If no file was uploaded but we have an ImageUrl, use that
                else if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    // If neither a file upload nor URL was provided, use a default image
                    product.ImageUrl = "/images/default_product.jpg";
                }
                // Otherwise, keep the provided ImageUrl
                
                product.FarmerId = farmer.Id;
                product.CreatedAt = DateTime.Now;

                _logger.LogInformation("Adding product to database: {@Product}", product);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product saved to database with ID: {ProductId}", product.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                ModelState.AddModelError("", "An error occurred while saving the product. Please try again.");
                return View(product);
            }
        }

        public async Task<IActionResult> ViewProducts(int? pageNumber)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int pageSize = 5; // Number of products per page
            var products = _context.Products
                .Where(p => p.FarmerId == farmer.Id)
                .OrderByDescending(p => p.CreatedAt);

            return View(await PaginatedList<Product>.CreateAsync(products, pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product, IFormFile productImage)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            // If no file is uploaded but an ImageUrl is provided, 
            // remove the productImage error from ModelState
            if ((productImage == null || productImage.Length == 0) && !string.IsNullOrEmpty(product.ImageUrl))
            {
                // Remove validation errors for productImage since we have a URL
                ModelState.Remove("productImage");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload if a file was submitted
                    if (productImage != null && productImage.Length > 0)
                    {
                        try
                        {
                            string imagePath = await _fileUploadService.UploadProductImageAsync(productImage);
                            existingProduct.ImageUrl = imagePath;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error uploading product image");
                            ModelState.AddModelError("productImage", "Error uploading image. Make sure it's a valid image file.");
                            return View(product);
                        }
                    }
                    // If no file was uploaded, check if a URL was provided
                    else if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        // Update with the provided URL
                        existingProduct.ImageUrl = product.ImageUrl;
                    }
                    // If no image was provided in either form, keep the existing one
                    
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Category = product.Category;
                    existingProduct.ProductionDate = product.ProductionDate;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewProducts));
            }
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product deleted: {ProductId}", id);
                return RedirectToAction(nameof(ViewProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {ProductId}", id);
                return RedirectToAction(nameof(ViewProducts));
            }
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(farmer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Farmer updatedFarmer, string userEmail)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Only update allowed fields
            farmer.Name = updatedFarmer.Name;
            farmer.Location = updatedFarmer.Location;
            farmer.Description = updatedFarmer.Description;
            farmer.PhoneNumber = updatedFarmer.PhoneNumber;
            
            // Update email if provided and changed
            if (!string.IsNullOrEmpty(userEmail) && userEmail != farmer.User.Email)
            {
                // Check if the email is already in use by another user
                if (await _context.Users.AnyAsync(u => u.Email == userEmail && u.Id != farmer.User.Id))
                {
                    TempData["ErrorMessage"] = "This email is already in use by another account.";
                    return RedirectToAction(nameof(Profile));
                }
                
                farmer.User.Email = userEmail;
            }

            try
            {
                _context.Update(farmer);
                if (farmer.User != null)
                {
                    _context.Update(farmer.User);
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farmer profile");
                TempData["ErrorMessage"] = "An error occurred while updating your profile.";
            }

            return RedirectToAction(nameof(Profile));
        }
    }
} 