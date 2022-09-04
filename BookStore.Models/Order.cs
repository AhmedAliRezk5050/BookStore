namespace BookStore.Models;

public class Order
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = null!;
    
    public ApplicationUser ApplicationUser { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string PostalCode { get; set; } = null!;
    
    public DateTime OrderDate { get; set; }
    
    public DateTime? ShippingDate { get; set; }

    public double OrderTotal { get; set; }

    public string? OrderStatus { get; set; }
    
    public string? PaymentStatus { get; set; }
    
    public string? TrackingNumber { get; set; }
    
    public string? Carrier { get; set; }

    public DateTime PaymentDate { get; set; }

    public DateTime PaymentDueDate { get; set; }

    public string? SessionId { get; set; }
    
    public string? PaymentIntentId { get; set; }
    
    
}