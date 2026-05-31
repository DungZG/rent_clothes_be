namespace thuegi_be.Contracts.Bookings;

public class BookingItemResponse
{
    public Guid CostumeId { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerDay { get; set; }
    public int TotalDays { get; set; }
    public decimal Subtotal { get; set; }
}
