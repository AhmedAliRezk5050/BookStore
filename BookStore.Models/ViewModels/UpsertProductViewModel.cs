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
    public class UpsertProductViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ISBN { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("List Price")]
        public double? ListPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for 1-50")]
        public double? Price { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for 51-100")]
        public double? Price50 { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for 100+")]
        public double? Price100 { get; set; }

        public IEnumerable<SelectListItem>? CategoriesSelectList { get; set; }

        public IEnumerable<SelectListItem>? CoverTypesSelectList { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        public int? CoverTypeId { get; set; }
    }
}
