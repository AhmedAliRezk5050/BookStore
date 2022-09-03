using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class ShoppingCart
{
    public int Id { get; set; }

    public int Count { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string ApplicationUserId { get; set; } = null!;

    public ApplicationUser ApplicationUser { get; set; } = null!;

    [NotMapped]
    public double Price { get; set; }

}