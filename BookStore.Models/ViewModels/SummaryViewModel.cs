using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookStore.Models.ViewModels;

public class SummaryViewModel
{
    public string Name { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string PostalCode { get; set; } = null!;
    
    public IEnumerable<ShoppingCart> Carts { get; set; } = Array.Empty<ShoppingCart>();

    public double TotalPrice { get; set; }
}