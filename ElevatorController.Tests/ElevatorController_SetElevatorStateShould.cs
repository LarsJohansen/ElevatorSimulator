namespace ElevatorController.Tests;

public class ElevatorController_SetElevatorStateShould
{

    [Fact]
    public void SetDestinationFloor_DestinationIs5_Shouldbe5()
    {
        // Arrange
        var elevator = new Elevator(1, 1);
        // Act
        elevator.AddDestinationFloor(5, true);
        // Assert
        Assert.Equal(5, elevator.NextFloor);
    }

    [Fact]
    public void SetDestinationFloor_DoorIsClosed_ShouldbeMoving()
    {
        // Arrange
        var elevator = new Elevator(1, 1);
        // Act
        elevator.AddDestinationFloor(5, true);
        // Assert
        Assert.Equal(ElevatorState.Moving, elevator.State);
    }

    [Fact]
    public void GetDirection_From3To5_ShouldBeUp()
    {
        // Arrange
        var elevator = new Elevator(1, 3);
        // Act
        elevator.AddDestinationFloor(5, true);
        // Assert
        Assert.Equal(Direction.Up, elevator.Direction);
    }
    [Fact]
    public void GetDirection_From5To3_ShouldBeDown()
    {
        // Arrange
        var elevator = new Elevator(1, 5);
        // Act
        elevator.AddDestinationFloor(3, false);
        // Assert
        Assert.Equal(Direction.Down, elevator.Direction);
    }

    [Fact]
    public void GetEstimatedTimeOfArrival_From3To5Idle_ShouldBe6()
    {
        // Arrange
        var elevator = new Elevator(1, 3);
        elevator.State = ElevatorState.Idle;
        var elevatorController = new ElevatorController(elevator, 3, 5);
        
        // Assert
        Assert.Equal(6, elevatorController.GetEstimatedTimeOfArrival(elevator, 5));
    }

    [Fact]
    public void GetEstimatedTimeOfArrival_From3WithDestination5_ShouldBe6()
    {
        // Arrange
        var elevator = new Elevator(1, 3);
        var elevatorController = new ElevatorController(elevator,3, 5);
        // Act
        elevator.AddDestinationFloor(5, true);
        // Assert
        Assert.Equal(6, elevatorController.GetEstimatedTimeOfArrival(elevator, 5));
    }

    [Fact]
    public void GetEstimatedTimeOfArrival_CalledFrom4WhenMoving2To3_Shouldbe11()
    {
        // Arrange
        var elevator = new Elevator(1, 2);
        var elevatorController = new ElevatorController(elevator, 3, 5);
        // Act
        elevator.AddDestinationFloor(3, true);
        elevatorController.CallElevator(elevator, 4, true);
        // Assert
        Assert.Equal(11, elevatorController.GetEstimatedTimeOfArrival(elevator, 4));
    }

    [Fact]
    public void CallElevator_WhenIdle_ShouldBeMoving()
    {
        // Arrange
        var elevator = new Elevator(1, 3);
        var elevatorController = new ElevatorController(elevator, 3, 5);
        // Act
        elevatorController.CallElevator(elevator, 5, true);
        // Assert
        Assert.Equal(ElevatorState.Moving, elevator.State);
    }

    [Theory]
    [InlineData(1, 8, 3, true, true, 3)]
    [InlineData(2, 4, 7, true, true, 4)]
    [InlineData(8, 2, 5, false, true, 2)]
    [InlineData(5, 1, 6, false, true, 1)]
    [InlineData(5, 3, 8, false, true, 3)]
    [InlineData(5, 8, 6, true, false, 8)]
    [InlineData(8, 2, 4, false, false, 4)]
    [InlineData(2, 8, 7, true, false, 8)]
    [InlineData(8, 5, 3, false, false, 5)]    
    public void CallElevator_WhenMovingAndCalled_DestinationShouldBe(int currentFloor,
                                                                     int destinationFloor,
                                                                     int callFloor,
                                                                     bool firstDirectionUp,
                                                                     bool secondDirectionUp,
                                                                     int expectedDestination) 
    {
        // Arrange
        var elevator = new Elevator(1, currentFloor);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevator.AddDestinationFloor(destinationFloor, firstDirectionUp);
        elevatorController.CallElevator(elevator, callFloor, secondDirectionUp);
        // Assert
        Assert.Equal(expectedDestination, elevator.NextFloor);
    }

    [Fact]
    public void CallElevator_SetNewHigherDestinationWhenAlreadyMoving_DestinationShouldNotChange() 
    {
        // Arrange
        var elevator = new Elevator(1, 8);
        elevator.AddDestinationFloor(5, true);
        elevator.State = ElevatorState.Moving;
        // Act
        elevator.AddDestinationFloor(10, false);
        // Assert
        Assert.Equal(5, elevator.NextFloor);
    }

    [Theory]
    [InlineData(1, new int[] {3,5,7,10,6}, true, 4, false, 8, true, new int[] {5,6,7,8,10})]
    public void CallElevator_WhenMovingAndCalledMultiple_DestinationsOrderShouldBe(int currentFloor,
                                                                     int[] destinationFloors,
                                                                     bool firstDirectionUp,
                                                                     int callFloor,
                                                                     bool secondDirectionUp,
                                                                     int thirdCallFloor,
                                                                     bool thirdDirectionUp,
                                                                     int[] expectedDestinations) 
    {
        // Arrange
        var elevator = new Elevator(1, currentFloor);
        var elevatorController = new ElevatorController(elevator);
        // Act
        elevator.AddDestinationFloors(destinationFloors.ToList(), firstDirectionUp);
        elevatorController.CallElevator(elevator, callFloor, secondDirectionUp);
        elevatorController.CallElevator(elevator, thirdCallFloor, thirdDirectionUp);
        // Assert
        Assert.Equal(expectedDestinations, elevator.DestinationFloors);
    }
    
}