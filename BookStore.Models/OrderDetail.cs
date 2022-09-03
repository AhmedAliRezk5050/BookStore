namespace BookStore.Models;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;
    
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    
    public double Price { get; set; }
}