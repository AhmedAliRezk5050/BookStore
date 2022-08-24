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
    public class ProductViewModel
    {
        public Product? Product { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoriesSelectList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? CoverTypesSelectList { get; set; }

        public string? ImagesFolderPath { get; set; }
    }
}
