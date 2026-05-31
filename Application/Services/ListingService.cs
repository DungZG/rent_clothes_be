using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common;
using Application.DTOs.Listings;
using Domain.Entities;

namespace Application.Services;

public class ListingService : IListingService
{
    private readonly IListingRepository _listingRepository;

    public ListingService(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public ListingResult Create(CreateListingCommand command)
    {
        if (command.ShopId == Guid.Empty)
        {
            throw new ServiceException("ShopId is required.", 400);
        }

        var listing = new RentalListing
        {
            Id = Guid.NewGuid(),
            ShopId = command.ShopId,
            Name = command.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim(),
            PricePerDay = decimal.Round(command.PricePerDay, 2),
            DepositAmount = decimal.Round(command.DepositAmount, 2),
            AvailableQuantity = command.AvailableQuantity,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _listingRepository.Add(listing);

        return new ListingResult
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
