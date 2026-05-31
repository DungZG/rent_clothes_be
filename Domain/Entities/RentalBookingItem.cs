namespace Domain.Entities;

public class RentalBookingItem
{
    public Guid CostumeId { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerDay { get; set; }
    public int TotalDays { get; set; }
    public decimal Subtotal { get; set; }
}
