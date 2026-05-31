namespace Application.DTOs.Bookings;

public class CreateBookingCommand
{
    public Guid CustomerId { get; set; }
    public Guid ShopId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<CreateBookingItemCommand> Items { get; set; } = new();
    public string? Notes { get; set; }
}
