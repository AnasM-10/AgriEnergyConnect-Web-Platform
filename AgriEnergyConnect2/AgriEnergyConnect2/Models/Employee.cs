
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Needed for IdentityUser

namespace AgriEnergyConnect2.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        // Link to IdentityUser
        [Required] // Still required as every employee needs an IdentityUser
        public string IdentityUserId { get; set; }

        [ForeignKey("IdentityUserId")]
        public IdentityUser IdentityUser { get; set; }

        [ForeignKey("Farmer")]
        public int? FarmerID { get; set; }

        public Farmer? Farmer { get; set; }

        // Make these nullable as per your requirement that they might not be filled on employee registration
        public string? FirstName { get; set; } // Made nullable
        public string? LastName { get; set; }  // Made nullable
        public string? ContactNumber { get; set; } // Made nullable
        public string? Email { get; set; } // Made nullable (can be taken from IdentityUser.Email if always available)
    }
}