namespace Application.Abstractions.Services;

using Application.DTOs.Listings;

public interface IListingService
{
    ListingResult Create(CreateListingCommand command);
}
