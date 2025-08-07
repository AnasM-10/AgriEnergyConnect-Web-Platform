using AgriEnergyConnect2.Data;
using AgriEnergyConnect2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

[Authorize(Roles = "Employee")]
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Action to view all farmers
    public IActionResult ViewFarmers()
    {
        var farmers = _dbContext.Farmers.ToList();
        return View(farmers);
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    // Action to add a new farmer (GET)
    public IActionResult AddFarmer()
    {
        return View();
    }

    // Action to add a new farmer (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFarmer(Farmer farmer)
    {
        if (ModelState.IsValid)
        {
            farmer.RegistrationDate = DateTime.Now;
            _dbContext.Farmers.Add(farmer);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Farmer added successfully!";
            return RedirectToAction(nameof(ViewFarmers)); // Redirect to ViewFarmers after adding
        }
        return View(farmer); // Return the view with validation errors if ModelState is invalid
    }

    // Action to view farmer products
    public IActionResult ViewFarmerProducts(int? farmerId)
    {
        if (farmerId == null || farmerId <= 0)
        {
            return NotFound();
        }

        var farmer = _dbContext.Farmers.FirstOrDefault(f => f.FarmerID == farmerId);
        if (farmer == null)
        {
            return NotFound();
        }

        ViewBag.FarmerName = $"{farmer.FirstName} {farmer.LastName}";
        var products = _dbContext.Products.Where(p => p.FarmerID == farmerId).ToList();
        return View(products);
    }

    // Action to filter products based on category and date range
    public IActionResult FilterProducts()
    {
        return View();
    }

    [HttpPost]
    public IActionResult FilterProducts(string category, DateTime? fromDate, DateTime? toDate)
    {
        var query = _dbContext.Products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(p => p.ProductionDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            toDate = toDate.Value.AddDays(1).AddSeconds(-1);
            query = query.Where(p => p.ProductionDate <= toDate.Value);
        }

        var filteredProducts = query.Include(p => p.Farmer).ToList();
        return View("ViewFilteredProducts", filteredProducts);
    }

    // Action to display filtered products
    public IActionResult ViewFilteredProducts(List<Product> model)
    {
        return View(model);
    }
}