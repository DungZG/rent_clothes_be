using thuegi_be.Contracts.Bookings;
using thuegi_be.Models;
using thuegi_be.Repositories;

namespace thuegi_be.Services;

public class BookingService : IBookingService
{
    private readonly IListingRepository _listingRepository;
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IListingRepository listingRepository, IBookingRepository bookingRepository)
    {
        _listingRepository = listingRepository;
        _bookingRepository = bookingRepository;
    }

    public BookingResponse Create(CreateBookingRequest request)
    {
        return CreateInternal(request);
    }

    public BookingResponse Checkout(CreateBookingRequest request)
    {
        return CreateInternal(request);
    }

    public BookingResponse GetDetail(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking is null)
        {
            throw new ServiceException($"Booking {bookingId} was not found.", StatusCodes.Status404NotFound);
        }

        return MapToResponse(booking);
    }

    public BookingResponse Cancel(Guid bookingId, string? reason)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking is null)
        {
            throw new ServiceException($"Booking {bookingId} was not found.", StatusCodes.Status404NotFound);
        }

        if (booking.Status == "cancelled")
        {
            throw new ServiceException($"Booking {bookingId} has already been cancelled.", StatusCodes.Status409Conflict);
        }

        if (booking.Status is "completed" or "refunded")
        {
            throw new ServiceException($"Booking {bookingId} cannot be cancelled from status {booking.Status}.", StatusCodes.Status409Conflict);
        }

        booking.Status = "cancelled";
        booking.Notes = string.IsNullOrWhiteSpace(reason)
            ? booking.Notes
            : string.IsNullOrWhiteSpace(booking.Notes)
                ? $"cancel_reason: {reason.Trim()}"
                : $"{booking.Notes}; cancel_reason: {reason.Trim()}";

        _bookingRepository.Update(booking);

        return MapToResponse(booking);
    }

    private BookingResponse CreateInternal(CreateBookingRequest request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ServiceException("CustomerId is required.", StatusCodes.Status400BadRequest);
        }

        if (request.ShopId == Guid.Empty)
        {
            throw new ServiceException("ShopId is required.", StatusCodes.Status400BadRequest);
        }

        if (request.EndDate < request.StartDate)
        {
            throw new ServiceException("EndDate must be on or after StartDate.", StatusCodes.Status400BadRequest);
        }

        var totalDays = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;
        if (totalDays <= 0)
        {
            throw new ServiceException("Booking duration must be at least one day.", StatusCodes.Status400BadRequest);
        }

        decimal subtotal = 0;
        decimal depositAmount = 0;
        var bookingItems = new List<RentalBookingItem>();

        foreach (var item in request.Items)
        {
            var listing = _listingRepository.GetById(item.CostumeId);
            if (listing is null)
            {
                throw new ServiceException($"Costume {item.CostumeId} was not found.", StatusCodes.Status404NotFound);
            }

            if (listing.ShopId != request.ShopId)
            {
                throw new ServiceException($"Costume {item.CostumeId} does not belong to shop {request.ShopId}.", StatusCodes.Status400BadRequest);
            }

            if (listing.AvailableQuantity < item.Quantity)
            {
                throw new ServiceException(
                    $"Costume {item.CostumeId} has only {listing.AvailableQuantity} item(s) available.",
                    StatusCodes.Status409Conflict);
            }

            var itemSubtotal = decimal.Round(listing.PricePerDay * item.Quantity * totalDays, 2);
            var itemDeposit = decimal.Round(listing.DepositAmount * item.Quantity, 2);

            subtotal += itemSubtotal;
            depositAmount += itemDeposit;

            bookingItems.Add(new RentalBookingItem
            {
                CostumeId = item.CostumeId,
                Quantity = item.Quantity,
                PricePerDay = listing.PricePerDay,
                TotalDays = totalDays,
                Subtotal = itemSubtotal
            });

            listing.AvailableQuantity -= item.Quantity;
            _listingRepository.Update(listing);
        }

        var booking = new RentalBooking
        {
            Id = Guid.NewGuid(),
            BookingCode = GenerateBookingCode(),
            CustomerId = request.CustomerId,
            ShopId = request.ShopId,
            Status = "pending",
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalDays = totalDays,
            Subtotal = decimal.Round(subtotal, 2),
            DepositAmount = decimal.Round(depositAmount, 2),
            TotalAmount = decimal.Round(subtotal + depositAmount, 2),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            Items = bookingItems
        };

        _bookingRepository.Add(booking);

        return MapToResponse(booking);
    }

    private static BookingResponse MapToResponse(RentalBooking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            BookingCode = booking.BookingCode,
            CustomerId = booking.CustomerId,
            ShopId = booking.ShopId,
            Status = booking.Status,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalDays = booking.TotalDays,
            Subtotal = booking.Subtotal,
            DepositAmount = booking.DepositAmount,
            TotalAmount = booking.TotalAmount,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt,
            Items = booking.Items.Select(x => new BookingItemResponse
            {
                CostumeId = x.CostumeId,
                Quantity = x.Quantity,
                PricePerDay = x.PricePerDay,
                TotalDays = x.TotalDays,
                Subtotal = x.Subtotal
            }).ToList()
        };
    }

    private static string GenerateBookingCode()
    {
        return $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}
