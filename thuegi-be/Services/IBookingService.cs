using thuegi_be.Contracts.Bookings;

namespace thuegi_be.Services;

public interface IBookingService
{
    BookingResponse Create(CreateBookingRequest request);
    BookingResponse Checkout(CreateBookingRequest request);
    BookingResponse GetDetail(Guid bookingId);
    BookingResponse Cancel(Guid bookingId, string? reason);
}
