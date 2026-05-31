namespace Application.DTOs.Listings;

public class CreateListingCommand
{
    public Guid ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal DepositAmount { get; set; }
    public int AvailableQuantity { get; set; }
}
