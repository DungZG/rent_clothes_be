namespace thuegi_be.Models;

public class RentalBooking
{
    public Guid Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid ShopId { get; set; }
    public string Status { get; set; } = "pending";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int TotalDays { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<RentalBookingItem> Items { get; set; } = new();
}
