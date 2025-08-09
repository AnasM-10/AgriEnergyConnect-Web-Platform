
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; 

namespace AgriEnergyConnect2.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

       
        [Required] 
        public string IdentityUserId { get; set; }

        [ForeignKey("IdentityUserId")]
        public IdentityUser IdentityUser { get; set; }

        [ForeignKey("Farmer")]
        public int? FarmerID { get; set; }

        public Farmer? Farmer { get; set; }

        
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }  
        public string? ContactNumber { get; set; } 
        public string? Email { get; set; } 
    }
}