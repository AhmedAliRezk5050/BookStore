namespace BookStore.Models.ViewModels;

public class CartViewModel
{
    public IEnumerable<ShoppingCart> Carts { get; set; } = Array.Empty<ShoppingCart>();

    public double TotalPrice { get; set; }
}