using ElevatorApi.Api.Exceptions;

namespace ElevatorApi.Tests.Services;

[Category("Unit")]
public class CarServiceTests
{
    private Mock<ICarRepository> Repository { get; set; }
    private CarService Sut { get; set; }

    [SetUp]
    public void Setup()
    {
        Repository = new();
        Sut = new(Repository.Object);
    }

    #region GetById

    [Test]
    public void GetById_CarNotFound_ReturnsNull()
    {
        Repository.Setup(s => s.GetById(1)).Returns(null as Car);
        Assert.That(Sut.GetById(1), Is.Null);
    }

    [Test]
    public void GetById_CarFound_ReturnsCar()
    {
        var car = new Car(1, 0, 0, 1);

        Repository.Setup(s => s.GetById(car.Id))
            .Returns(car);

        Assert.That(Sut.GetById(car.Id), Is.EqualTo(car));
    }

    #endregion

    #region AddStop

    [Test]
    public void AddStop_CarNotFound_ReturnsNull()
    {
        byte carId = 1;
        Repository.Setup(s => s.GetById(carId))
            .Returns(null as Car);

        Assert.Throws<CarNotFoundException>(() =>
            Sut.AddStop(carId, 1));
    }

    [Test]
    public void AddStop_CarFound_AddsStop()
    {
        var car = new Car(1, 0, -1, 10);
        Repository.Setup(s => s.GetById(car.Id))
            .Returns(car);

        var actual = Sut.AddStop(car.Id, 2);
        Assert.That(actual.Stops, Does.Contain(2));
    }

    #endregion
}