using AwesomeAssertions;
using Sdk.Client.Extensions;
using Xunit;

namespace Sdk.Client.Tests;

public sealed class TypeExtensionsTests
{
    [Fact]
    public void Should_return_base_type()
    {
        // Arrange
        var roadCarType = typeof(RoadCar);
        var carOpenGenericType = typeof(Car<>);

        // Act
        var baseType = roadCarType.GetBaseTypeRecursive(carOpenGenericType);

        // Assert
        baseType.Should().Be<Car<PetrolEngine>>();
    }

    [Fact]
    public void Should_return_base_type_from_recursion()
    {
        // Arrange
        var roadCarType = typeof(RoadCar);
        var motorVehicleOpenGenericType = typeof(MotorVehicle<>);

        // Act
        var baseType = roadCarType.GetBaseTypeRecursive(motorVehicleOpenGenericType);

        // Assert
        baseType.Should().Be<MotorVehicle<PetrolEngine>>();
    }

    [Fact]
    public void Should_throw_exception_when_unrelated_base_type_is_passed()
    {
        // Arrange
        var roadCarType = typeof(RoadCar);

        // Act
        var call = roadCarType.Invoking(subject => subject.GetBaseTypeRecursive(typeof(PetrolEngine)));

        // Assert
        call.Should().Throw<InvalidOperationException>().WithMessage("Type '*' does not inherit from '*'");
    }

    private sealed class PetrolEngine;

    private sealed class RoadCar : Car<PetrolEngine>
    {
        public override PetrolEngine Engine { get; } = new();
    }

    private abstract class Car<TEngine> : MotorVehicle<TEngine>;

    private abstract class MotorVehicle<TEngine>
    {
        public abstract TEngine Engine { get; }
    }
}
