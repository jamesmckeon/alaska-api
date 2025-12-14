using ElevatorApi.Api.Dal;
using ElevatorApi.Api.Exceptions;

namespace ElevatorApi.Api.Services;

public class CarService : ICarService
{
    private ICarRepository CarRepository { get; }

    public CarService(ICarRepository carRepository)
    {
        CarRepository = carRepository ??
                        throw new ArgumentNullException(nameof(carRepository));
    }

    public Car? GetById(byte id)
    {
        return CarRepository.GetById(id);
    }

    public Car AddStop(byte carId, sbyte floorNumber)
    {
        var car = CarRepository.GetById(carId);

        if (car == null)
        {
            throw new CarNotFoundException(carId);
        }

        car.AddStop(floorNumber);

        return car;
    }
}