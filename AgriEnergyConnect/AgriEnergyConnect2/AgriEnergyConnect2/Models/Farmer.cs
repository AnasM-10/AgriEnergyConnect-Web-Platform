using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 

namespace AgriEnergyConnect2.Models
{
    public class Farmer
    {
        public int FarmerID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        // navigation properties to the Farmer model:
        public ICollection<Employee>? Employees { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}