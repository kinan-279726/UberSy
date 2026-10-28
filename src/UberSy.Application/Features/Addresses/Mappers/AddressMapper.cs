using System;
using System.Collections.Generic;
using System.Text;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Domain.Addresses;

namespace UberSy.Application.Features.Addresses.Mappers;
public static class AddressMapper
{
    public static AddressDto ToDto(this TbAddress entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AddressDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Street = entity.Street,
            City = entity.City,
            Latitude = entity.Boundary.Y,
            Longitude = entity.Boundary.X,
            Label = entity.Label,
            Floor = entity.Floor,
            Phone = entity.Phone,
            IsDefault = entity.IsDefault,
        };
    }

    public static List<AddressDto> ToDtos(this IEnumerable<TbAddress> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }
}