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
        elevatorController.CallElevator(4, true);
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
        elevatorController.CallElevator(5, true);
        // Assert
        Assert.Equal(ElevatorState.Moving, elevator.State);
    }

    [Theory]
    [InlineData(1, 8, 3, true, 3)]
    [InlineData(2, 4, 7, true, 4)]
    [InlineData(8, 2, 5, true, 2)]
    [InlineData(5, 1, 6, true, 1)]
    [InlineData(5, 3, 8, true, 3)]
    [InlineData(5, 8, 6, false, 8)]
    [InlineData(8, 2, 4, false, 4)]
    [InlineData(2, 8, 7, false, 8)]
    [InlineData(8, 5, 3, false, 5)]    
    public void CallElevator_WhenMovingAndCalledUp_DestinationShouldBe(int currentFloor, int destinationFloor, int callFloor, bool up, int expectedDestination) 
    {
        // Arrange
        var elevator = new Elevator(1, currentFloor, 0);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevatorController.SetDestinationFloor(destinationFloor);
        elevatorController.CallElevator(callFloor, up);
        // Assert
        Assert.Equal(expectedDestination, elevator.DestinationFloor);
    }
    
}