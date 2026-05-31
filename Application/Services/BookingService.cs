using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common;
using Application.DTOs.Bookings;
using Domain.Entities;
using Domain.Rules;

namespace Application.Services;

public class BookingService : IBookingService
{
    private readonly IListingRepository _listingRepository;
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IListingRepository listingRepository, IBookingRepository bookingRepository)
    {
        _listingRepository = listingRepository;
        _bookingRepository = bookingRepository;
    }

    public BookingResult Create(CreateBookingCommand command)
    {
        return CreateInternal(command);
    }

    public BookingResult Checkout(CreateBookingCommand command)
    {
        return CreateInternal(command);
    }

    public BookingResult GetDetail(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking is null)
        {
            throw new ServiceException($"Booking {bookingId} was not found.", 404);
        }

        return MapToResult(booking);
    }

    public BookingResult Cancel(Guid bookingId, string? reason)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking is null)
        {
            throw new ServiceException($"Booking {bookingId} was not found.", 404);
        }

        if (booking.Status == "cancelled")
        {
            throw new ServiceException($"Booking {bookingId} has already been cancelled.", 409);
        }

        if (!BookingPolicy.CanCancel(booking.Status))
        {
            throw new ServiceException($"Booking {bookingId} cannot be cancelled from status {booking.Status}.", 409);
        }

        booking.Status = "cancelled";
        booking.Notes = string.IsNullOrWhiteSpace(reason)
            ? booking.Notes
            : string.IsNullOrWhiteSpace(booking.Notes)
                ? $"cancel_reason: {reason.Trim()}"
                : $"{booking.Notes}; cancel_reason: {reason.Trim()}";

        _bookingRepository.Update(booking);

        return MapToResult(booking);
    }

    private BookingResult CreateInternal(CreateBookingCommand command)
    {
        if (command.CustomerId == Guid.Empty)
        {
            throw new ServiceException("CustomerId is required.", 400);
        }

        if (command.ShopId == Guid.Empty)
        {
            throw new ServiceException("ShopId is required.", 400);
        }

        if (!BookingPolicy.IsValidDateRange(command.StartDate, command.EndDate))
        {
            throw new ServiceException("EndDate must be on or after StartDate.", 400);
        }

        var totalDays = BookingPolicy.CalculateTotalDays(command.StartDate, command.EndDate);
        if (totalDays <= 0)
        {
            throw new ServiceException("Booking duration must be at least one day.", 400);
        }

        decimal subtotal = 0;
        decimal depositAmount = 0;
        var bookingItems = new List<RentalBookingItem>();

        foreach (var item in command.Items)
        {
            var listing = _listingRepository.GetById(item.CostumeId);
            if (listing is null)
            {
                throw new ServiceException($"Costume {item.CostumeId} was not found.", 404);
            }

            if (listing.ShopId != command.ShopId)
            {
                throw new ServiceException($"Costume {item.CostumeId} does not belong to shop {command.ShopId}.", 400);
            }

            if (!BookingPolicy.HasSufficientQuantity(listing.AvailableQuantity, item.Quantity))
            {
                throw new ServiceException(
                    $"Costume {item.CostumeId} has only {listing.AvailableQuantity} item(s) available.",
                    409);
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
            CustomerId = command.CustomerId,
            ShopId = command.ShopId,
            Status = "pending",
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            TotalDays = totalDays,
            Subtotal = decimal.Round(subtotal, 2),
            DepositAmount = decimal.Round(depositAmount, 2),
            TotalAmount = decimal.Round(subtotal + depositAmount, 2),
            Notes = string.IsNullOrWhiteSpace(command.Notes) ? null : command.Notes.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            Items = bookingItems
        };

        _bookingRepository.Add(booking);

        return MapToResult(booking);
    }

    private static BookingResult MapToResult(RentalBooking booking)
    {
        return new BookingResult
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
            Items = booking.Items.Select(x => new BookingItemResult
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
