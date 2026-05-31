using Application.Abstractions.Services;
using Application.Common;
using Application.DTOs.Listings;
using Microsoft.AspNetCore.Mvc;
using thuegi_be.Contracts.Listings;

namespace thuegi_be.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController : ControllerBase
{
    private readonly IListingService _listingService;

    public ListingsController(IListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public ActionResult<ListingResponse> Create([FromBody] CreateListingRequest request)
    {
        if (!HasAuthenticatedRole(out var role))
        {
            return Problem(detail: "Missing X-Role header.", statusCode: StatusCodes.Status401Unauthorized);
        }

        if (!string.Equals(role, "shop_owner", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(detail: "Only shop_owner can create listing.", statusCode: StatusCodes.Status403Forbidden);
        }

        try
        {
            var result = _listingService.Create(new CreateListingCommand
            {
                ShopId = request.ShopId,
                Name = request.Name,
                Description = request.Description,
                PricePerDay = request.PricePerDay,
                DepositAmount = request.DepositAmount,
                AvailableQuantity = request.AvailableQuantity
            });

            return CreatedAtAction(nameof(Create), new { id = result.Id }, new ListingResponse
            {
                Id = result.Id,
                ShopId = result.ShopId,
                Name = result.Name,
                Description = result.Description,
                PricePerDay = result.PricePerDay,
                DepositAmount = result.DepositAmount,
                AvailableQuantity = result.AvailableQuantity,
                CreatedAt = result.CreatedAt
            });
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
}
