using thuegi_be.Contracts.Listings;

namespace thuegi_be.Services;

public interface IListingService
{
    ListingResponse Create(CreateListingRequest request);
}
