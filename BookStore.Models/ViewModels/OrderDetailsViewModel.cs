using BookStore.Utility;

namespace BookStore.Models.ViewModels;

public class OrderDetailsViewModel
{
   public bool IsAdminOrEmployee { get; set; }
   public int Id { get; set; }

   public string Name { get; set; } = null!;
   
   public string PhoneNumber { get; set; } = null!;
   
   public string StreetAddress { get; set; } = null!;
   
   public string City { get; set; } = null!;
   
   public string State { get; set; } = null!;
   
   public string PostalCode { get; set; } = null!;
   
   public string Email { get; set; } = null!;
   
   public double OrderTotal { get; set; }
   
   public DateTime OrderDate { get; set; }
   
   public string? Carrier { get; set; }
   
   public string? TrackingNumber { get; set; }
   
   public DateTime? ShippingDate { get; set; }
   
   public DateTime PaymentDate { get; set; }
   
   public DateTime PaymentDueDate { get; set; }

   public string? SessionId { get; set; }
   
   public string? PaymentIntentId { get; set; }
   
   public string? PaymentStatus { get; set; }
   
   public string? OrderStatus { get; set; } = null!;
   
   public IEnumerable<OrderDetail>? OrderDetails { get; set; }
}