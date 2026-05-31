using System.ComponentModel.DataAnnotations;

namespace thuegi_be.Contracts.Bookings;

public class CancelBookingRequest
{
    [StringLength(1000)]
    public string? Reason { get; set; }
}
