namespace ElevatorController;

public class ElevatorController
{
    public static int ElevatorSpeedInSecondsPerFloor { get; private set; }
    public static int DoorOpenTimeInSeconds { get; private set; }
    public List<Elevator> Elevators { get; set; } = new();

    public ElevatorController(
        List<Elevator> elevators,
        int elevatorSpeedInSecondsPerFloor = 3,
        int doorOpenTimeInSeconds = 5
    )
    {
        ElevatorSpeedInSecondsPerFloor = elevatorSpeedInSecondsPerFloor;
        DoorOpenTimeInSeconds = doorOpenTimeInSeconds;
        Elevators = elevators;
    }

    public Elevator CallElevator(int fromFloor, bool up)
    {
        var closestElevator = FindClosestElevator(fromFloor, up);
        closestElevator.AddDestinationFloor(fromFloor, up);
        return closestElevator;
    }

    private Elevator FindClosestElevator(int floor, bool up)
    {
        var direction = up ? Direction.Up : Direction.Down;
        //Find the closest idle elevator
        Elevator? closestIdleElevator = Elevators
            .Where(x => x.State == ElevatorState.Idle)
            .OrderBy(x => Math.Abs(x.CurrentFloor - floor))
            .FirstOrDefault();
        // Find the closest elevator that is moving in the same direction and has not passed the requested floor
        Elevator? closestMovingElevator = Elevators
            .Where(
                x =>
                    x.Direction == direction
                    && (
                        x.Direction == Direction.Down
                            ? x.CurrentFloor - floor > 0
                            : x.CurrentFloor - floor < 0
                    )
            )
            .OrderBy(
                x => x.Direction == Direction.Down ? x.CurrentFloor - floor : floor - x.CurrentFloor
            )
            .FirstOrDefault();
        // Check if the closest idle elevator is closer than the closest moving elevator and retun the one that is closest
        int distancetoIdleElevator =
            closestIdleElevator != null
                ? Math.Abs(closestIdleElevator.CurrentFloor - floor)
                : int.MaxValue;
        int distanceToMovingElevator =
            closestMovingElevator != null
                ? Math.Abs(closestMovingElevator.CurrentFloor - floor)
                : int.MaxValue;

        if (closestIdleElevator != null || closestMovingElevator != null)
        {
            return distancetoIdleElevator < distanceToMovingElevator
                ? closestIdleElevator!
                : closestMovingElevator!;
        }
        // If no elevators going in the same direction and no idle elevators, return the elevator that will be closest when it reaches the top or bottom floor
        // This has some issues. If more floors are added to the elevators queue while it is moving, it will not take those floors into account
        Dictionary<Elevator, int> elevatorsWithLastFloorInCurrentDirection = new();
        foreach (var elevator in Elevators)
        {
            int lastDestinationFloor = 0;
            var tempDestinationFloors = elevator.DestinationFloors.ToList();
            if (tempDestinationFloors.Count > 0)
            {
                lastDestinationFloor = tempDestinationFloors.Last();
            }
            if (!elevatorsWithLastFloorInCurrentDirection.ContainsKey(elevator))
            {
                elevatorsWithLastFloorInCurrentDirection.Add(elevator, lastDestinationFloor);
            }
        }

        return elevatorsWithLastFloorInCurrentDirection
            .OrderBy(x => Math.Abs(x.Value - floor))
            .FirstOrDefault()
            .Key;
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
            // var tempDestinationFloors = elevator.DestinationFloors.ToList();
            return (
                    Math.Abs(elevator.CurrentFloor - elevator.NextFloor)
                    * ElevatorSpeedInSecondsPerFloor
                )
                + DoorOpenTimeInSeconds
                + (Math.Abs(elevator.NextFloor - floor) * ElevatorSpeedInSecondsPerFloor);
        }
    }
}
