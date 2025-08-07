
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using AgriEnergyConnect2.Models;
using AgriEnergyConnect2.Data;

namespace AgriEnergyConnect2.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _db = db;

            // Initialize non-nullable properties to avoid warnings
            Input = new InputModel();
            ExternalLogins = new List<AuthenticationScheme>();
            ReturnUrl = string.Empty;
            RoleList = new List<SelectListItem>();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        public List<SelectListItem> RoleList { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Please select a role.")]
            public string Role { get; set; }

            // Farmer specific fields - these will be conditionally validated
            [Display(Name = "First Name")]
            public string? FarmerFirstName { get; set; }

            [Display(Name = "Last Name")]
            public string? FarmerLastName { get; set; }

            [Display(Name = "Contact Number")]
            [Phone]
            public string? FarmerContactNumber { get; set; }

            [Display(Name = "Address")]
            public string? FarmerAddress { get; set; }

            // Employee specific fields - these can be collected later or made optional
            [Display(Name = "First Name")]
            public string? EmployeeFirstName { get; set; }

            [Display(Name = "Last Name")]
            public string? EmployeeLastName { get; set; }

            [Display(Name = "Contact Number")]
            [Phone]
            public string? EmployeeContactNumber { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Ensure roles exist (consider moving this to SeedData.Initialize or an app startup event)
            if (!_roleManager.RoleExistsAsync("Farmer").Result)
                await _roleManager.CreateAsync(new IdentityRole("Farmer"));
            if (!_roleManager.RoleExistsAsync("Employee").Result)
                await _roleManager.CreateAsync(new IdentityRole("Employee"));
            if (!_roleManager.RoleExistsAsync("Admin").Result)
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            RoleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Role --", Disabled = true, Selected = true },
                new SelectListItem { Value = "Farmer", Text = "Farmer" },
                new SelectListItem { Value = "Employee", Text = "Employee" }
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Re-populate RoleList in case of ModelState errors to display dropdown correctly
            RoleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Role --", Disabled = true, Selected = true },
                new SelectListItem { Value = "Farmer", Text = "Farmer" },
                new SelectListItem { Value = "Employee", Text = "Employee" }
            };

            // Manually validate fields based on selected role
            if (Input.Role == "Farmer")
            {
                if (string.IsNullOrWhiteSpace(Input.FarmerFirstName)) ModelState.AddModelError("Input.FarmerFirstName", "Please enter your first name.");
                if (string.IsNullOrWhiteSpace(Input.FarmerLastName)) ModelState.AddModelError("Input.FarmerLastName", "Please enter your last name.");
                if (string.IsNullOrWhiteSpace(Input.FarmerContactNumber)) ModelState.AddModelError("Input.FarmerContactNumber", "Please enter your contact number.");
                if (string.IsNullOrWhiteSpace(Input.FarmerAddress)) ModelState.AddModelError("Input.FarmerAddress", "Please enter your address.");
            }
            // For Employee, we're making FirstName, LastName, ContactNumber optional at registration
            // If you wanted them required for Employee too, you would add similar validation here.

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, Input.Role);

                    if (Input.Role == "Farmer")
                    {
                        var farmer = new Farmer
                        {
                            FirstName = Input.FarmerFirstName!, // Use null-forgiving operator since we validated it
                            LastName = Input.FarmerLastName!,
                            ContactNumber = Input.FarmerContactNumber!,
                            Address = Input.FarmerAddress!,
                            Email = user.Email,
                            RegistrationDate = DateTime.Now
                        };
                        _db.Farmers.Add(farmer);
                        await _db.SaveChangesAsync();
                        _logger.LogInformation($"Farmer {farmer.FirstName} {farmer.LastName} created and associated.");
                    }
                    else if (Input.Role == "Employee")
                    {
                        // For Employee, populate fields if provided, otherwise they will be null
                        var employee = new Employee
                        {
                            IdentityUserId = user.Id,
                            FirstName = Input.EmployeeFirstName,
                            LastName = Input.EmployeeLastName,
                            ContactNumber = Input.EmployeeContactNumber,
                            Email = user.Email, // Use IdentityUser's email, or Input.EmployeeEmail if you added it
                            FarmerID = null // Employees are not necessarily tied to a Farmer at registration
                        };
                        _db.Employees.Add(employee);
                        await _db.SaveChangesAsync();
                        _logger.LogInformation($"Employee {user.Email} created and associated.");
                    }

                    // Sign in the user after successful registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, re-render form with errors
            return Page();
        }
    }
}