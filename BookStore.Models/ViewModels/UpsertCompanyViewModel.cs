using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class UpsertCompanyViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }

        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
