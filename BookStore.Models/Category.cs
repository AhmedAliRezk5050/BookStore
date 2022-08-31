using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        
        // ? and Required   to avoid '' in server side validation
        [Required]
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100.")]
        public int? DisplayOrder { get; set; }

        [Required]
        [DisplayName("Creation Date")]
        public DateTime? CreatedDateTime { get; set; }
    }
}