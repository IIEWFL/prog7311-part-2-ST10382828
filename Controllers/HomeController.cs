using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Prog7311_Part2.Models;
using System.Security.Claims;

namespace Prog7311_Part2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var role = User.FindFirstValue("Role");
            if (role == UserRole.Farmer.ToString())
            {
                return RedirectToAction("Index", "Farmer");
            }
            else if (role == UserRole.Employee.ToString())
            {
                return RedirectToAction("Index", "Employee");
            }
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
