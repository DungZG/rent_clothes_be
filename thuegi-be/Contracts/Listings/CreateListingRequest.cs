using System.ComponentModel.DataAnnotations;

namespace thuegi_be.Contracts.Listings;

public class CreateListingRequest
{
    [Required]
    public Guid ShopId { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal PricePerDay { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal DepositAmount { get; set; }

    [Range(1, 1000)]
    public int AvailableQuantity { get; set; }
}
