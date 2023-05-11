using AutoMapper;
using Liewald.TruckService.Application.Dtos;
using Liewald.TruckService.Domain.Models;

namespace Liewald.TruckService.Application.Extensions;

public static class DtoExtensions
{
    private static readonly IMapper _mapper;

    static DtoExtensions()
    {
        MapperConfiguration config = new(config =>
        {
            config.CreateMap<Driver, DriverDto>();
        });

        _mapper = config.CreateMapper();
    }

    public static DriverDto ToDto(this Driver driver) => _mapper.Map<DriverDto>(driver);
}
