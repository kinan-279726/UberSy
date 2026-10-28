using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Features.Restaurants.Commands.ApproveRestaurant;
using UberSy.Application.Features.Restaurants.Commands.RejectRestaurant;
using UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;

namespace UberSy.Api.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController(ISender sender) : ApiController
{
    [HttpPost("driver/{driverId:guid}/approve")]
    public async Task<IActionResult> ApproveDriver([FromRoute] Guid driverId)
    {
        return Ok();
    }

    [HttpPatch("restaurant/{restaurantId:guid}/commission-reate")]
    public async Task<IActionResult> UpdateCommissionRate([FromBody] UpdateCommissionRateCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPatch("restaurant/{restaurantId:guid}/reject")]
    public async Task<IActionResult> RejectRestaurant([FromRoute] Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RejectRestaurantCommand(restaurantId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPatch("restaurant/{restaurantId:guid}/approve")]
    public async Task<IActionResult> ApproveRestaurant([FromRoute] Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ApproveRestaurantCommand(restaurantId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

}
