using System.Collections.ObjectModel;
using ElevatorApi.Api.Config;
using ElevatorApi.Api.Dal;
using ElevatorApi.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace ElevatorApi.Api.Services;

public class CarService : ICarService
{
    private ICarRepository CarRepository { get; }
    private IOptions<ElevatorSettings> Settings { get; }

    public CarService(ICarRepository carRepository, IOptions<ElevatorSettings> settings)
    {
        CarRepository = carRepository ??
                        throw new ArgumentNullException(nameof(carRepository));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public Car? GetById(byte id)
    {
        return CarRepository.GetById(id);
    }

    public Car AddStop(byte carId, sbyte floorNumber)
    {
        var car = CarRepository.GetById(carId) ??
                  throw new CarNotFoundException(carId);
        car.AddStop(floorNumber);
        return car;
    }

    public Car MoveCar(byte carId)
    {
        var car = CarRepository.GetById(carId) ??
                  throw new CarNotFoundException(carId);
        car.MoveNext();
        return car;
    }

    public Car CallCar(sbyte floorNumber)
    {
        //TODO REFACTOR!!!!!!!!!!!!!
        
        ValidateFloor(floorNumber);

        var cars = CarRepository.GetAll()
            .ToList();

        Car? car = cars.FirstOrDefault(c =>
            c.State == CarState.Idle ||
            c.NextFloor == floorNumber ||
            c.CurrentFloor == floorNumber);

        if (car == null)
        {
            var enroute = GetEnRouteToFloor(cars.ToList(), floorNumber);

            if (enroute.Count == 1)
            {
                car = enroute.Single();
            }
            else if (enroute.Count > 1)
            {
                var fewestStops = GetFewestStops(enroute);

                if (fewestStops.Count == 1)
                {
                    car = fewestStops.Single();
                }
                else
                {
                    // multiple cars headed towards floorNumber
                    // return the one with the closest current floor
                    car = GetWithClosestCurrentFloor(cars.ToList(), floorNumber)
                        .OrderBy(c => c.Id).First();
                }
            }
            else
            {
                // no cars currently headed in direction of floorNumber
                // return the one with the least stops or the closest
                // last floor
                var fewestStops = GetFewestStops(cars);

                if (fewestStops.Count == 1)
                {
                    car = fewestStops.Single();
                }
                else
                {
                    // no cars headed towards floorNumber
                    // return the one with the closest last stop
                    car = GetWithClosestLastStop(cars.ToList(), floorNumber)
                        .OrderBy(c => c.Id).First();
                }
            }
        }
        
        car.AddStop(floorNumber);
        return car;
    }

    /// <summary>
    /// returns items in <paramref name="cars"/> with the fewest stops
    /// </summary>
    private static List<Car> GetEnRouteToFloor(List<Car> cars, sbyte floorNumber)
    {
        var enroute = cars.Where(c => (c.State == CarState.Ascending && floorNumber >= c.CurrentFloor)
                                      || (c.State == CarState.Descending && floorNumber <= c.CurrentFloor))
            .ToList();

        return GetFewestStops(enroute);
    }

    /// <summary>
    /// returns the items in <paramref name="cars"/> with the fewest stops
    /// </summary>
    private static List<Car> GetFewestStops(List<Car> cars)
    {
        if (cars.Count == 0)
        {
            return Enumerable.Empty<Car>()
                .ToList();
        }

        var minStops = cars.Select(s => s.Stops.Count)
            .Min(stops => stops);

        return cars.Where(c => c.Stops.Count == minStops).ToList();
    }

    /// <summary>
    /// returns all items with a last stop that is the minimum distance from <paramref name="floorNumber"/>
    /// </summary>
    private static List<Car> GetWithClosestLastStop(List<Car> cars, sbyte floorNumber)
    {
        var lastStops = cars.Select(c => new
        {
            Car = c, Diff =
                (c.State == CarState.Ascending ? c.Stops.Max() : c.Stops.Min())
                - floorNumber
        }).ToList();

        return lastStops.Where(ls => Math.Abs(ls.Diff) == lastStops.Min(d => Math.Abs(d.Diff)))
            .Select(ls => ls.Car)
            .ToList();
    }

    /// <summary>
    /// returns all items with a current floor closest to <paramref name="floorNumber"/>
    /// </summary>
    /// <remarks>assumes that all items in <paramref name="cars"/> are
    /// headed towards <paramref name="floorNumber"/></remarks>
    private static List<Car> GetWithClosestCurrentFloor(List<Car> cars, sbyte floorNumber)
    {
        var minDiff = cars
            .Select(c => Math.Abs(c.CurrentFloor - floorNumber))
            .Min();

        return cars.Where(c => Math.Abs(c.CurrentFloor - floorNumber) == minDiff)
            .ToList();
    }

    private void ValidateFloor(sbyte floorNumber)
    {
        if (floorNumber < Settings.Value.MinFloor || floorNumber > Settings.Value.MaxFloor)
        {
            throw new ArgumentOutOfRangeException(nameof(floorNumber),
                $"floorNumber must be between {Settings.Value.MinFloor} and " +
                $"{Settings.Value.MaxFloor}");
        }
    }
}