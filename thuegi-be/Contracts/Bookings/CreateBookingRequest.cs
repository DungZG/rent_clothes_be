using System.ComponentModel.DataAnnotations;

namespace thuegi_be.Contracts.Bookings;

public class CreateBookingRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid ShopId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateBookingItemRequest> Items { get; set; } = new();

    [StringLength(1000)]
    public string? Notes { get; set; }
}
