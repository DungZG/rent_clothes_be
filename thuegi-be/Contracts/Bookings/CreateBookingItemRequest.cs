using System.ComponentModel.DataAnnotations;

namespace thuegi_be.Contracts.Bookings;

public class CreateBookingItemRequest
{
    [Required]
    public Guid CostumeId { get; set; }

    [Range(1, 50)]
    public int Quantity { get; set; }
}
