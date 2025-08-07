using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AgriEnergyConnect2.Models;
using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnect2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Farmer"))
                return RedirectToAction("Dashboard", "Farmer");
            else if (roles.Contains("Employee"))
                return RedirectToAction("Dashboard", "Employee");
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
