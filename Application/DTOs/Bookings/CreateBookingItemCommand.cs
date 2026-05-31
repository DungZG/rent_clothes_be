namespace Application.DTOs.Bookings;

public class CreateBookingItemCommand
{
    public Guid CostumeId { get; set; }
    public int Quantity { get; set; }
}
