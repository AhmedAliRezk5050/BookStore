﻿using System.ComponentModel.DataAnnotations;

namespace BookStoreWeb.Models
{
    public class Category
    {
        // Id is PK by convention
        //for other names, [Key] must be used
        //[Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
