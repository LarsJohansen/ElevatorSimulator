namespace ElevatorController.Tests;

public class ElevatorController_MultipleElevators
{
    private readonly ElevatorController _elevatorController;

    public ElevatorController_MultipleElevators()
    {
        var elevators = new List<Elevator>()
        {
            new Elevator(1, 1),
            new Elevator(2, 3),
            new Elevator(3, 5),
            new Elevator(4, 7),
            new Elevator(5, 9),
        };
        _elevatorController = new ElevatorController(elevators, 1, 1);
    }

    [Fact]
    public void CallElevator_FromFloor1_ShouldBeElevator1()
    {
        // Act
        var elevator = _elevatorController.CallElevator(1, true);
        // Assert
        Assert.Equal(1, elevator.Id);
    }

    [Fact]
    public void CallElevator_FromFloor3_ShouldBeElevator2()
    {
        // Act
        var elevator = _elevatorController.CallElevator(3, true);
        // Assert
        Assert.Equal(2, elevator.Id);
    }

    [Fact]
    public void CallElevator_FromFloor5GoingDown_ShouldBeElevator4()
    {
        // Elevator 1,2,3 are all going up
        // Elevator 4 and 5 are going down
        // We are are calling elevator from floor 5 going down
        // Elevator 4 is closer to floor 5 and should be picked

        // Arrange
        var elevator1 = _elevatorController.Elevators.Find(e => e.Id == 1);
        elevator1?.AddDestinationFloor(6, true);
        var elevator2 = _elevatorController.Elevators.Find(e => e.Id == 2);
        elevator2?.AddDestinationFloor(9, true);
        var elevator3 = _elevatorController.Elevators.Find(e => e.Id == 3);
        elevator3?.AddDestinationFloor(8, true);
        var elevator4 = _elevatorController.Elevators.Find(e => e.Id == 4);
        elevator4?.AddDestinationFloor(1, false);
        var elevator5 = _elevatorController.Elevators.Find(e => e.Id == 5);
        elevator5?.AddDestinationFloor(1, false);
        // Act
        var elevator = _elevatorController.CallElevator(5, false);
        // Assert
        Assert.Equal(4, elevator.Id);
    }

    [Fact]
    public void CallElevator_Elevators1FloorDown_ShouldPickIdle()
    {
        // Arrange
        var elevatorController = new ElevatorController(
            new List<Elevator>()
            {
                new Elevator(1, 5),
                new Elevator(2, 5),
                new Elevator(3, 1),
                new Elevator(4, 1),
                new Elevator(5, 1),
            },
            1,
            1
        );
        var elevator1 = elevatorController.Elevators.Find(e => e.Id == 1);
        elevator1?.AddDestinationFloor(1, false);
        // Act
        var elevator = elevatorController.CallElevator(6, false);
        // Assert
        Assert.Equal(2, elevator.Id);
    }

    [Fact]
    public void CallElevator_NoElevatorsInSameDirectionOrIdle_ShouldPickClosestAfterDirectionChange()
    {
        var elevatorController = new ElevatorController(
            new List<Elevator>()
            {
                new Elevator(1, 1),
                new Elevator(2, 2),
                new Elevator(3, 3),
                new Elevator(4, 4),
                new Elevator(5, 5),
            },
            1,
            1
        );
        // All elevators except 2 are going up to 8 and 9
        foreach (var ele in elevatorController.Elevators)
        {
            if (ele.Id != 2)
            {
                ele.AddDestinationFloors(new List<int>() { 8, 9 }, true);
            }
        }
        // Elevator 2 will first go up to 5 and 6 and then down to 2 and 1
        var elevator2 = elevatorController.Elevators.Find(e => e.Id == 2);
        elevator2?.AddDestinationFloors(new List<int>() { 5, 6 }, true);
        elevator2?.AddDestinationFloors(new List<int>() { 1, 2 }, false);
        // Elevator 1 will go up to 8 and 9 the down to 2 and 1
        var elevator1 = elevatorController.Elevators.Find(e => e.Id == 1);
        elevator1?.AddDestinationFloors(new List<int>() { 1, 2 }, false);
        // We call elevator from floor 4 going down
        var elevator = elevatorController.CallElevator(4, false);
        // Elevator 2 should be picked because it is closest after direction change
        Assert.Equal(2, elevator.Id);
    }
}
