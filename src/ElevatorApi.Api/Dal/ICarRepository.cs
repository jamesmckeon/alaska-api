using ElevatorApi.Models;

namespace ElevatorApi.Tests.Services;

public interface ICarRepository
{
    IReadOnlyList<Car> GetAll();
    Car? GetById(byte id);
}