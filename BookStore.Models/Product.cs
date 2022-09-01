using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ISBN { get; set; } = null!;

        public string Author { get; set; } = null!;

        public double ListPrice { get; set; }

        public double Price { get; set; }

        public double Price50 { get; set; }

        public double Price100 { get; set; }

        public string ImageUrl { get; set; } = null!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public int CoverTypeId { get; set; }

        public CoverType CoverType { get; set; } = null!;
    }
}
