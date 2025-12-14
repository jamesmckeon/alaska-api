using ElevatorApi.Models;

namespace ElevatorApi.Tests.Services;

public interface IFloorsRespository
{
    IReadOnlyList<Floor> GetAll();
    Floor GetByFloorNumber(byte number);
}