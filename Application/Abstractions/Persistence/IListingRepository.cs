using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IListingRepository
{
    RentalListing Add(RentalListing listing);
    RentalListing? GetById(Guid id);
    void Update(RentalListing listing);
}
