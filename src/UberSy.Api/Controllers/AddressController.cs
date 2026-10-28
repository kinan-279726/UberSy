using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Features.Addresses.Commands.CreateAddress;
using UberSy.Application.Features.Addresses.Commands.DeleateAddress;
using UberSy.Application.Features.Addresses.Commands.UpdateAddressDetails;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Contract.Requests.Addresses;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/addresses")]
[ApiVersion("1.0")]
[Authorize]
public sealed class AddressesController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves my addresses.")]
    [EndpointDescription("Returns all addresses of the authenticated user.")]
    [EndpointName("GetMyAddresses")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAddressesByUserQuery(), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{addressId:guid}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves an address.")]
    [EndpointDescription("Returns one of my addresses by id.")]
    [EndpointName("GetAddressById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid addressId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAddressByIdQuery(addressId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Creates an address.")]
    [EndpointDescription("Adds a new address with coordinates for the authenticated user.")]
    [EndpointName("CreateAddress")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAddressCommand(
            request.Street, request.City, request.Latitude, request.Longitude,
            request.Label, request.Floor, request.Phone);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            response => CreatedAtRoute("GetAddressById", new { version = "1.0", addressId = response.Id }, response),
            Problem);
    }

    [HttpPut("{addressId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates an address.")]
    [EndpointDescription("Updates the details and coordinates of one of my addresses.")]
    [EndpointName("UpdateAddress")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(
        Guid addressId,
        [FromBody] UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAddressDetailsCommand(
            addressId, request.Street, request.City, request.Latitude, request.Longitude,
            request.Label, request.Floor, request.Phone);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{addressId:guid}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Marks an address as default.")]
    [EndpointDescription("The previous default address is unset.")]
    [EndpointName("MarkAddressAsDefault")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> MarkAsDefault(Guid addressId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new MarkAddressAsDefaultCommand(addressId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("{addressId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Deletes an address.")]
    [EndpointDescription("Removes one of my addresses.")]
    [EndpointName("DeleteAddress")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(Guid addressId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteAddressCommand(addressId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}