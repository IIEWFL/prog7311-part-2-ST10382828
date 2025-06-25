using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using Prog7311_Part2.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Prog7311_Part2.Controllers
{
    [Authorize(Policy = "EmployeeOnly")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(AppDbContext context, ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Loading farmers for employee dashboard");
            var farmers = await _context.Farmers
                .Include(f => f.Products)
                .ToListAsync();
            
            _logger.LogInformation("Found {FarmerCount} farmers with a total of {ProductCount} products", 
                farmers.Count, farmers.Sum(f => f.Products.Count));
            
            return View(farmers);
        }

        public IActionResult AddFarmer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(RegisterFarmerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already taken");
                    return View(model);
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered");
                    return View(model);
                }

                // Factory Pattern Implementation:
                // This demonstrates another implementation of the Factory Pattern
                // where an Employee creates a Farmer user with an associated profile.
                // This aligns with the report's mention of the Factory Pattern for user creation.
                
                // Create user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = UserRole.Farmer,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create farmer profile
                var farmer = new Farmer
                {
                    Name = model.FarmerName,
                    Location = model.Location,
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now
                };

                _context.Farmers.Add(farmer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> ViewFarmerProducts(int id, int? pageNumber)
        {
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farmer == null)
            {
                return NotFound();
            }

            ViewBag.FarmerName = farmer.Name;
            ViewBag.FarmerId = farmer.Id;

            const int pageSize = 5; // Number of products per page
            var products = _context.Products
                .Where(p => p.FarmerId == id)
                .OrderByDescending(p => p.CreatedAt);

            return View(await PaginatedList<Product>.CreateAsync(products, pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> FilterProducts()
        {
            var farmers = await _context.Farmers.ToListAsync();
            ViewBag.Farmers = farmers;
            ViewBag.Categories = Enum.GetValues(typeof(ProductCategory));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FilterProducts(int? farmerId, DateTime? startDate, DateTime? endDate, ProductCategory? category, int? pageNumber)
        {
            var query = _context.Products.AsQueryable();

            if (farmerId.HasValue)
            {
                query = query.Where(p => p.FarmerId == farmerId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate <= endDate.Value);
            }

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            // Include Farmer and order by creation date
            query = query.Include(p => p.Farmer).OrderByDescending(p => p.CreatedAt);
            
            var farmers = await _context.Farmers.ToListAsync();
            ViewBag.Farmers = farmers;
            ViewBag.Categories = Enum.GetValues(typeof(ProductCategory));
            
            // Save the filter criteria in ViewBag to maintain form state
            ViewBag.SelectedFarmerId = farmerId;
            ViewBag.SelectedStartDate = startDate;
            ViewBag.SelectedEndDate = endDate;
            ViewBag.SelectedCategory = category;

            // Paginate the results
            const int pageSize = 10; // Number of products per page
            var paginatedResults = await PaginatedList<Product>.CreateAsync(query, pageNumber ?? 1, pageSize);

            return View("FilterResults", paginatedResults);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(p => p.Id == id);

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
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Employee updatedEmployee, string userEmail)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Only update allowed fields
            employee.Name = updatedEmployee.Name;
            employee.Department = updatedEmployee.Department;
            employee.EmployeeNumber = updatedEmployee.EmployeeNumber;
            employee.PhoneNumber = updatedEmployee.PhoneNumber;
            
            // Update email if provided and changed
            if (!string.IsNullOrEmpty(userEmail) && userEmail != employee.User.Email)
            {
                // Check if the email is already in use by another user
                if (await _context.Users.AnyAsync(u => u.Email == userEmail && u.Id != employee.User.Id))
                {
                    TempData["ErrorMessage"] = "This email is already in use by another account.";
                    return RedirectToAction(nameof(Profile));
                }
                
                employee.User.Email = userEmail;
            }

            try
            {
                _context.Update(employee);
                if (employee.User != null)
                {
                    _context.Update(employee.User);
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee profile");
                TempData["ErrorMessage"] = "An error occurred while updating your profile.";
            }

            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> DeleteFarmer(int id)
        {
            var farmer = await _context.Farmers
                .Include(f => f.User)
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        [HttpPost, ActionName("DeleteFarmer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFarmerConfirmed(int id)
        {
            var farmer = await _context.Farmers
                .Include(f => f.User)
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farmer == null)
            {
                return NotFound();
            }

            try
            {
                // Delete associated products
                _context.Products.RemoveRange(farmer.Products);
                
                // Delete farmer record
                _context.Farmers.Remove(farmer);
                
                // Delete user account
                if (farmer.User != null)
                {
                    _context.Users.Remove(farmer.User);
                }
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Farmer '{farmer.Name}' has been successfully deleted.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farmer: {FarmerId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the farmer.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 