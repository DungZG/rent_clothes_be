using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBookingRepository
{
    RentalBooking Add(RentalBooking booking);
    RentalBooking? GetById(Guid bookingId);
    void Update(RentalBooking booking);
}
