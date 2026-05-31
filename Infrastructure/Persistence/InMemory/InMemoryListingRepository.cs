using Application.Abstractions.Persistence;
using Domain.Entities;
using System.Collections.Concurrent;

namespace Infrastructure.Persistence.InMemory;

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
