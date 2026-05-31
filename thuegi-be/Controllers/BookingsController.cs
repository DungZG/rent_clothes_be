using Application.Abstractions.Services;
using Application.Common;
using Application.DTOs.Bookings;
using Microsoft.AspNetCore.Mvc;
using thuegi_be.Contracts.Bookings;

namespace thuegi_be.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<BookingResponse> Create([FromBody] CreateBookingRequest request)
    {
        if (!HasAuthenticatedRole(out var role))
        {
            return Problem(detail: "Missing X-Role header.", statusCode: StatusCodes.Status401Unauthorized);
        }

        if (!string.Equals(role, "customer", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(detail: "Only customer can create booking.", statusCode: StatusCodes.Status403Forbidden);
        }

        try
        {
            var result = _bookingService.Create(ToCreateBookingCommand(request));
            return CreatedAtAction(nameof(GetDetail), new { bookingId = result.Id }, ToBookingResponse(result));
        }
        catch (ServiceException ex)
        {
            return Problem(detail: ex.Message, statusCode: ex.StatusCode);
        }
    }

    [HttpPost("checkout")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<BookingResponse> Checkout([FromBody] CreateBookingRequest request)
    {
        if (!HasAuthenticatedRole(out var role))
        {
            return Problem(detail: "Missing X-Role header.", statusCode: StatusCodes.Status401Unauthorized);
        }

        if (!string.Equals(role, "customer", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(detail: "Only customer can checkout booking.", statusCode: StatusCodes.Status403Forbidden);
        }

        try
        {
            var result = _bookingService.Checkout(ToCreateBookingCommand(request));
            return CreatedAtAction(nameof(GetDetail), new { bookingId = result.Id }, ToBookingResponse(result));
        }
        catch (ServiceException ex)
        {
            return Problem(detail: ex.Message, statusCode: ex.StatusCode);
        }
    }

    [HttpGet("{bookingId:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<BookingResponse> GetDetail(Guid bookingId)
    {
        if (!HasAuthenticatedRole(out var role))
        {
            return Problem(detail: "Missing X-Role header.", statusCode: StatusCodes.Status401Unauthorized);
        }

        if (!IsAllowedDetailRole(role))
        {
            return Problem(detail: "Role is not allowed to view booking detail.", statusCode: StatusCodes.Status403Forbidden);
        }

        try
        {
            return Ok(ToBookingResponse(_bookingService.GetDetail(bookingId)));
        }
        catch (ServiceException ex)
        {
            return Problem(detail: ex.Message, statusCode: ex.StatusCode);
        }
    }

    [HttpPost("{bookingId:guid}/cancel")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<BookingResponse> Cancel(Guid bookingId, [FromBody] CancelBookingRequest request)
    {
        if (!HasAuthenticatedRole(out var role))
        {
            return Problem(detail: "Missing X-Role header.", statusCode: StatusCodes.Status401Unauthorized);
        }

        if (!string.Equals(role, "customer", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(role, "shop_owner", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(detail: "Only customer or shop_owner can cancel booking.", statusCode: StatusCodes.Status403Forbidden);
        }

        try
        {
            return Ok(ToBookingResponse(_bookingService.Cancel(bookingId, request.Reason)));
        }
        catch (ServiceException ex)
        {
            return Problem(detail: ex.Message, statusCode: ex.StatusCode);
        }
    }

    private bool HasAuthenticatedRole(out string role)
    {
        role = string.Empty;

        if (!Request.Headers.TryGetValue("X-Role", out var roleHeader))
        {
            return false;
        }

        role = roleHeader.ToString();
        return !string.IsNullOrWhiteSpace(role);
    }

    private static bool IsAllowedDetailRole(string role)
    {
        return string.Equals(role, "customer", StringComparison.OrdinalIgnoreCase)
            || string.Equals(role, "shop_owner", StringComparison.OrdinalIgnoreCase)
            || string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase);
    }

    private static CreateBookingCommand ToCreateBookingCommand(CreateBookingRequest request)
    {
        return new CreateBookingCommand
        {
            CustomerId = request.CustomerId,
            ShopId = request.ShopId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Notes = request.Notes,
            Items = request.Items.Select(item => new CreateBookingItemCommand
            {
                CostumeId = item.CostumeId,
                Quantity = item.Quantity
            }).ToList()
        };
    }

    private static BookingResponse ToBookingResponse(BookingResult result)
    {
        return new BookingResponse
        {
            Id = result.Id,
            BookingCode = result.BookingCode,
            CustomerId = result.CustomerId,
            ShopId = result.ShopId,
            Status = result.Status,
            StartDate = result.StartDate,
            EndDate = result.EndDate,
            TotalDays = result.TotalDays,
            Subtotal = result.Subtotal,
            DepositAmount = result.DepositAmount,
            TotalAmount = result.TotalAmount,
            Notes = result.Notes,
            CreatedAt = result.CreatedAt,
            Items = result.Items.Select(item => new BookingItemResponse
            {
                CostumeId = item.CostumeId,
                Quantity = item.Quantity,
                PricePerDay = item.PricePerDay,
                TotalDays = item.TotalDays,
                Subtotal = item.Subtotal
            }).ToList()
        };
    }
}
