namespace Application.Abstractions.Services;

using Application.DTOs.Bookings;

public interface IBookingService
{
    BookingResult Create(CreateBookingCommand command);
    BookingResult Checkout(CreateBookingCommand command);
    BookingResult GetDetail(Guid bookingId);
    BookingResult Cancel(Guid bookingId, string? reason);
}
