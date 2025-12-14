using ElevatorApi.Models;

namespace ElevatorApi.Tests.Services;

public interface ICarService
{
    Car AssignCartoFloor(byte floorNumber);
}