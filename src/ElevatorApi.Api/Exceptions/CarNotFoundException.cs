namespace ElevatorApi.Api.Exceptions;

public class CarNotFoundException : Exception
{
    public CarNotFoundException(byte carId) : base($"A car with id {carId} wasn't found.")
    {
    }

    public CarNotFoundException(string message) : base(message)
    {
    }

    public CarNotFoundException()
    {
    }

    public CarNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}