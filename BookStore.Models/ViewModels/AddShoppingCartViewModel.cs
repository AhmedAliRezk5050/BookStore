using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookStore.Models.ViewModels
{
    public class AddShoppingCartViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be at least {1}")]
        [DisplayName("Product count")]
        public int? Count { get; set; }

        [ValidateNever] public Product Product { get; set; } = null!;
    }
}