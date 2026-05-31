using thuegi_be.Contracts.Listings;
using thuegi_be.Models;
using thuegi_be.Repositories;

namespace thuegi_be.Services;

public class ListingService : IListingService
{
    private readonly IListingRepository _listingRepository;

    public ListingService(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public ListingResponse Create(CreateListingRequest request)
    {
        if (request.ShopId == Guid.Empty)
        {
            throw new ServiceException("ShopId is required.", StatusCodes.Status400BadRequest);
        }

        var listing = new RentalListing
        {
            Id = Guid.NewGuid(),
            ShopId = request.ShopId,
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            PricePerDay = decimal.Round(request.PricePerDay, 2),
            DepositAmount = decimal.Round(request.DepositAmount, 2),
            AvailableQuantity = request.AvailableQuantity,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _listingRepository.Add(listing);

        return new ListingResponse
        {
            Id = listing.Id,
            ShopId = listing.ShopId,
            Name = listing.Name,
            Description = listing.Description,
            PricePerDay = listing.PricePerDay,
            DepositAmount = listing.DepositAmount,
            AvailableQuantity = listing.AvailableQuantity,
            CreatedAt = listing.CreatedAt
        };
    }
}
