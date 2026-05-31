using thuegi_be.Models;

namespace thuegi_be.Repositories;

public interface IBookingRepository
{
    RentalBooking Add(RentalBooking booking);
    RentalBooking? GetById(Guid bookingId);
    void Update(RentalBooking booking);
}
