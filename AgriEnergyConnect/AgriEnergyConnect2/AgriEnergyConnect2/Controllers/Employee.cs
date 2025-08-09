using AgriEnergyConnect2.Data;
using AgriEnergyConnect2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AgriEnergyConnect2.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult ViewFarmers()
        {
            var farmers = _dbContext.Farmers.ToList();
            return View(farmers);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AddFarmer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    farmer.RegistrationDate = DateTime.Now;
                    _dbContext.Farmers.Add(farmer);
                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Farmer '{farmer.FirstName} {farmer.LastName}' added successfully!";
                    return RedirectToAction(nameof(ViewFarmers));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding farmer: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the farmer. Please try again.");
                }
            }
            return View(farmer);
        }

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

        public async Task<IActionResult> FilterProducts(string category, DateTime? fromDate, DateTime? toDate)
        {
            var query = _dbContext.Products.Include(p => p.Farmer).AsQueryable();

           
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
                toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.ProductionDate <= toDate.Value);
            }

      
            var filteredProducts = await query.ToListAsync();

           
            ViewBag.Category = category;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd"); 
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");   

            // Return the view with the list of products
            return View(filteredProducts);
        }

       
    }
}