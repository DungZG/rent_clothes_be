using System.Collections.Concurrent;
using thuegi_be.Models;

namespace thuegi_be.Repositories;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly ConcurrentDictionary<Guid, RentalBooking> _bookings = new();

    public RentalBooking Add(RentalBooking booking)
    {
        _bookings[booking.Id] = booking;
        return booking;
    }

    public RentalBooking? GetById(Guid bookingId)
    {
        _bookings.TryGetValue(bookingId, out var booking);
        return booking;
    }

    public void Update(RentalBooking booking)
    {
        _bookings[booking.Id] = booking;
    }
}
