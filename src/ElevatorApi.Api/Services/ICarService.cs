using ElevatorApi.Api.Models;

namespace ElevatorApi.Api.Services;

public interface ICarService
{
    Car? GetById(byte id);
    /// <summary>
    /// Adds a floor to a car's stops
    /// </summary>
    /// <param name="carId">the id of a car</param>
    /// <param name="floorNumber">the floor to add to the car's stops</param>
    /// <returns>the affected car</returns>
    Car AddStop(byte carId, sbyte floorNumber);
}