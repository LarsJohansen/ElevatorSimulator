namespace ElevatorController;
public class ElevatorController
{
    public static int ElevatorSpeedInSecondsPerFloor { get; private set; }
    public static int DoorOpenTimeInSeconds { get; private set; } = 5;
    public ElevatorController(Elevator elevator, int elevatorSpeedInSecondsPerFloor = 3, int doorOpenTimeInSeconds = 5)
    {
        ElevatorSpeedInSecondsPerFloor = elevatorSpeedInSecondsPerFloor;
        DoorOpenTimeInSeconds = doorOpenTimeInSeconds;
    }
    public void CallElevator(Elevator elevator, int fromFloor, bool up)
    {
        elevator.AddDestinationFloor(fromFloor, up);
    }

    public int GetEstimatedTimeOfArrival(Elevator elevator, int floor)
    {
        if (elevator.State == ElevatorState.Idle)
        {
            return Math.Abs(elevator.CurrentFloor - floor) * ElevatorSpeedInSecondsPerFloor;
        }
        else if (elevator.NextFloor == floor) 
        {
            return Math.Abs(elevator.CurrentFloor - floor) * ElevatorSpeedInSecondsPerFloor;
        }
        else 
        {
            return Math.Abs(elevator.CurrentFloor - elevator.NextFloor)
                   * ElevatorSpeedInSecondsPerFloor
                   + DoorOpenTimeInSeconds
                   + Math.Abs(elevator.NextFloor - floor)
                   * ElevatorSpeedInSecondsPerFloor;
        }
    }
    
}
