using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        // all properties will be nullable in DB
        // because IdentityUser doesn't have them
        [Required]
        public string Name { get; set; } = null!;
        
        public string? StreetAddress { get; set; } = null!;
                     
        public string? City { get; set; } = null!;
                     
        public string? State { get; set; } = null!;
                     
        public string? PostalCode { get; set; } = null!;
    }
}
