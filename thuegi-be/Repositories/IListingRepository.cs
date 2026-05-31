using thuegi_be.Models;

namespace thuegi_be.Repositories;

public interface IListingRepository
{
    RentalListing Add(RentalListing listing);
    RentalListing? GetById(Guid id);
    void Update(RentalListing listing);
}
