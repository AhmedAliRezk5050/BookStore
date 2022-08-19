using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreWeb.Models
{
    public class Category
    {
        // Id is PK by convention
        //for other names, [Key] must be used
        //[Key]
        public int Id { get; set; }

        // [Required] is not needed because NRT is enabled
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