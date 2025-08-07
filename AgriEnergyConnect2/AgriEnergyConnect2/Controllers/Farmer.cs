using AgriEnergyConnect2.Data;
using AgriEnergyConnect2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AgriEnergyConnect2.Controllers
{
    // Ensure the user is authenticated and belongs to the "Farmer" role
    [Authorize(Roles = "Farmer")]
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public FarmerController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Farmer Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Displays the form to add a new farmer (Note: This route is typically for Admins/Employees, not Farmers themselves to add new farmers)
        public IActionResult AddFarmer()
        {
            return View();
        }

        // POST: Handles the submission of the Add Farmer form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(Farmer farmer)
        {
            // Set the registration date automatically
            farmer.RegistrationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _dbContext.Farmers.Add(farmer);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Farmer added successfully!"; // Optional: for success messages
                return RedirectToAction(nameof(ViewFarmers)); // This usually means viewing all farmers, which an Employee would do.
            }
            // If ModelState is not valid, return to the form with validation errors
            return View(farmer);
        }

        // GET: Displays a list of all farmers (Typically for Employees/Admins)
        public IActionResult ViewFarmers()
        {
            // Retrieve all farmers from the database
            var farmers = _dbContext.Farmers.ToList();
            return View(farmers);
        }

        // GET: Displays the form to add a new product
        public IActionResult AddProduct()
        {
            // Ensure the product object is initialized to avoid null reference issues
            // and to provide default values for date fields if needed.
            return View(new Product());
        }

        // POST: Handles the submission of the Add Product form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            // Set the added date automatically
            product.AddedDate = DateTime.Now;

            // Crucially, ensure FarmerID is not being bound from the form directly for security.
            // It will be populated from the current user's FarmerID.
            ModelState.Remove(nameof(product.FarmerID));
            ModelState.Remove(nameof(product.Farmer)); // Also remove the navigation property from model state

            if (ModelState.IsValid)
            {
                // Retrieve the current logged-in user (IdentityUser)
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found. Please log in.");
                    return View(product);
                }

                // Find the Farmer associated with the logged-in user's email using ToLower()
                var farmer = await _dbContext.Farmers
                    .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

                if (farmer == null)
                {
                    ModelState.AddModelError(string.Empty, "Farmer profile not found for the logged-in user. Please ensure your farmer profile is complete or contact support.");
                    return View(product);
                }

                product.FarmerID = farmer.FarmerID; // Assign the retrieved Farmer's ID to the product

                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction(nameof(ViewProducts)); // Redirect to ViewProducts after adding
            }

            // If ModelState is not valid, return to the form with validation errors
            // The validation summary and individual field validation messages in the view will now display.
            return View(product);
        }

        // GET: Displays products associated with the current logged-in farmer
        public async Task<IActionResult> ViewProducts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle case where user is not logged in (though [Authorize] should prevent this)
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Find the Farmer associated with the logged-in user's email using ToLower()
            var farmer = await _dbContext.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null)
            {
                TempData["InfoMessage"] = "No farmer profile found for the logged-in user. Please ensure your farmer profile is complete.";
                return View(new List<Product>());
            }

            var products = await _dbContext.Products
                .Include(p => p.Farmer) // Eager load Farmer details
                .Where(p => p.FarmerID == farmer.FarmerID)
                .ToListAsync();

            return View(products);
        }

        [AllowAnonymous] // This action is likely for Employees to view products of a specific farmer
        public async Task<IActionResult> ViewFarmerProducts(int farmerId)
        {
            var farmer = await _dbContext.Farmers.FirstOrDefaultAsync(f => f.FarmerID == farmerId);
            if (farmer == null)
            {
                TempData["ErrorMessage"] = "Farmer not found.";
                return RedirectToAction(nameof(ViewFarmers)); // Or some other appropriate redirect
            }

            var products = await _dbContext.Products
                .Include(p => p.Farmer)
                .Where(p => p.FarmerID == farmerId)
                .ToListAsync();

            ViewData["FarmerName"] = $"{farmer.FirstName} {farmer.LastName}'s Products";
            return View("ViewProducts", products); // Reuse the ViewProducts view
        }


        // GET: Edit Product
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            // Authorization check for Edit: Ensure product belongs to the current farmer before showing the edit form
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _dbContext.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null || product.FarmerID != farmer.FarmerID)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this product.";
                return RedirectToAction(nameof(ViewProducts));
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, [Bind("ProductID,Name,Category,ProductionDate,Description")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found. Please log in.");
                        return View(product);
                    }

                    var farmer = await _dbContext.Farmers
                        .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

                    var originalProduct = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == id);

                    if (farmer == null || originalProduct == null || originalProduct.FarmerID != farmer.FarmerID)
                    {
                        ModelState.AddModelError(string.Empty, "You are not authorized to edit this product.");
                        return View(product);
                    }

                    product.FarmerID = originalProduct.FarmerID;
                    product.AddedDate = originalProduct.AddedDate;

                    _dbContext.Update(product);
                    await _dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction(nameof(ViewProducts));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Products.Any(e => e.ProductID == product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex) // Catch other database update exceptions
                {
                    // Log the error for debugging
                    ModelState.AddModelError(string.Empty, $"An error occurred while saving: {ex.Message}");
                    return View(product);
                }
            }
            else
            {
                // Log validation errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(product); // Return the view with validation errors
            }
        }

        // GET: Farmer/DeleteProduct/{id}
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Authorization check: Ensure the product belongs to the current farmer
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _dbContext.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null || product.FarmerID != farmer.FarmerID)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this product.";
                return RedirectToAction(nameof(ViewProducts));
            }

            return View(product); // You'll need a confirmation view (DeleteProduct.cshtml)
        }

        // POST: Farmer/DeleteProduct/{id}
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Authorization check (again, to prevent unauthorized POST requests)
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _dbContext.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null || product.FarmerID != farmer.FarmerID)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this product.";
                return RedirectToAction(nameof(ViewProducts));
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(ViewProducts));
        }
    }
}