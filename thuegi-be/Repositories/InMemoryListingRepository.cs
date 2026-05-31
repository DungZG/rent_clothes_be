using System.Collections.Concurrent;
using thuegi_be.Models;

namespace thuegi_be.Repositories;

public class InMemoryListingRepository : IListingRepository
{
    private readonly ConcurrentDictionary<Guid, RentalListing> _listings = new();

    public RentalListing Add(RentalListing listing)
    {
        _listings[listing.Id] = listing;
        return listing;
    }

    public RentalListing? GetById(Guid id)
    {
        _listings.TryGetValue(id, out var listing);
        return listing;
    }

    public void Update(RentalListing listing)
    {
        _listings[listing.Id] = listing;
    }
}
