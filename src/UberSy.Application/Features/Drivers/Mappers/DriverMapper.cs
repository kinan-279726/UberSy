using System;
using System.Collections.Generic;
using System.Text;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Discounts;
using UberSy.Domain.Drivers;

namespace UberSy.Application.Features.Drivers.Mappers
{
    public static class DriverMapper
    {
        public static DriverDto ToDto(this TbDriver entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new DriverDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                VehicleType = entity.VehicleType,
                ServiceTier = entity.ServiceTier,
                CarType = entity.CarType,
                NationalId = entity.NationalId,
                PhotoUrl = entity.PhotoUrl,
                LicenseNumber = entity.LicenseNumber,
                PlateNumber = entity.PlateNumber,
                Model = entity.Model,
                Color = entity.Color,
                Status = entity.Status,
                IsApproved = entity.IsApproved,
            };
        }

        public static List<DriverDto> ToDtos(this List<TbDriver> entities)
        {
            return [.. entities.Select(e => e.ToDto())];
        }
    }
}
