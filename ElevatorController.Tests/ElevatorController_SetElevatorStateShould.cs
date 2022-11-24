namespace ElevatorController.Tests;

public class ElevatorController_SetElevatorStateShould
{

    [Fact]
    public void SetDestinationFloor_DestinationIs5_Shouldbe5()
    {
        // Arrange
        var elevator = new Elevator(1, 1, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.SetDestinationFloor(5);
        // Assert
        Assert.Equal(5, elevator.DestinationFloor);
    }

    [Fact]
    public void SetDestinationFloor_DoorIsClosed_ShouldbeMoving()
    {
        // Arrange
        var elevator = new Elevator(1, 1, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.SetDestinationFloor(5);
        // Assert
        Assert.Equal(ElevatorState.Moving, elevator.State);
    }

    [Fact]
    public void GetDirection_From3To5_ShouldBeUp()
    {
        // Arrange
        var elevator = new Elevator(1, 3, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.SetDestinationFloor(5);
        // Assert
        Assert.Equal(Direction.Up, elevatorController.GetDirection());
    }
    [Fact]
    public void GetDirection_From5To3_ShouldBeDown()
    {
        // Arrange
        var elevator = new Elevator(1, 5, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.SetDestinationFloor(3);
        // Assert
        Assert.Equal(Direction.Down, elevatorController.GetDirection());
    }

    [Fact]
    public void GetEstimatedTimeOfArrival_From3To5Idle_ShouldBe6()
    {
        // Arrange
        var elevator = new Elevator(1, 3, 0);
        elevator.State = ElevatorState.Idle;
        var elevatorController = new ElevatorController(elevator);
        elevatorController.ElevatorSpeedInSecondsPerFloor = 3;
        
        // Assert
        Assert.Equal(6, elevatorController.GetEstimatedTimeOfArrival(5));
    }

    [Fact]
    public void GetEstimatedTimeOfArrival_From3WithDestination5_ShouldBe6()
    {
        // Arrange
        var elevator = new Elevator(1, 3, 0);
        var elevatorController = new ElevatorController(elevator);
        elevatorController.ElevatorSpeedInSecondsPerFloor = 3;
        // Act
        elevatorController.SetDestinationFloor(5);
        // Assert
        Assert.Equal(6, elevatorController.GetEstimatedTimeOfArrival(5));
    }

    [Fact]
    public void GetEstimatedTimeOfArrival_CalledFrom4WhenMoving2To3_Shouldbe11()
    {
        // Arrange
        var elevator = new Elevator(1, 2, 0);
        var elevatorController = new ElevatorController(elevator);
        elevatorController.ElevatorSpeedInSecondsPerFloor = 3;
        // Act
        elevatorController.SetDestinationFloor(3);
        elevatorController.CallElevator(4);
        // Assert
        Assert.Equal(11, elevatorController.GetEstimatedTimeOfArrival(4));
    }

    [Fact]
    public void CallElevator_WhenIdle_ShouldBeMoving()
    {
        // Arrange
        var elevator = new Elevator(1, 3, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.CallElevator(5);
        // Assert
        Assert.Equal(ElevatorState.Moving, elevator.State);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CallElevator_WhenIdle_DestinationShouldBeSet(int value) 
    {
        // Arrange
        var elevator = new Elevator(1, 3, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.CallElevator(value);
        // Assert
        Assert.Equal(value, elevator.DestinationFloor);
    }

    [Fact]
    public void CallElevator_WhenMoving_NextDestinationShouldBe5() 
    {
        // Arrange
        var elevator = new Elevator(1, 3, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.SetDestinationFloor(3);
        elevatorController.CallElevator(5);
        // Assert
        Assert.Equal(5, elevator.NextDestinationFloor);
    }

    // [Fact]
    // public void EmergencyStop_WhenMoving_ShouldBeFailure()
    // {
    //     // Arrange
    //     var elevator = new Elevator(1, 3, 0);
    //     var elevatorController = new ElevatorController(elevator);
    //     // Act
    //     elevatorController.SetDestinationFloor(5);
    //     elevatorController.EmergencyStop();
    //     // Assert
    //     Assert.Equal(ElevatorState.Failure, elevator.State);
    // }
}