namespace thuegi_be.Contracts.Listings;

public class ListingResponse
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal DepositAmount { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
