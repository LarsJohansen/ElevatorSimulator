namespace ElevatorController;

public class ElevatorController
{
    public static int ElevatorSpeedInSecondsPerFloor { get; private set; }
    public static int DoorOpenTimeInSeconds { get; private set; }
    public List<Elevator> Elevators { get; set; }

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
            .Where(x => x.State == ElevatorState.Idle).MinBy(x => Math.Abs(x.CurrentFloor - floor));
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
            ).MinBy(x => x.Direction == Direction.Down ? x.CurrentFloor - floor : floor - x.CurrentFloor);
        // Check if the closest idle elevator is closer than the closest moving elevator and retun the one that is closest
        int distanceToIdleElevator =
            closestIdleElevator != null
                ? Math.Abs(closestIdleElevator.CurrentFloor - floor)
                : int.MaxValue;
        int distanceToMovingElevator =
            closestMovingElevator != null
                ? Math.Abs(closestMovingElevator.CurrentFloor - floor)
                : int.MaxValue;

        if (closestIdleElevator != null || closestMovingElevator != null)
        {
            return distanceToIdleElevator < distanceToMovingElevator
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

    public int GetEstimatedTimeOfArrival(Elevator elevator, int floor, bool wantedDirectionUp)
    {
        // If the elevator is idle or next floor is the same as we are calculating,
        // the estimated time of arrival is the time it takes to get to the floor
        if (elevator.State == ElevatorState.Idle || elevator.NextFloor == floor)
        {
            return Math.Abs(elevator.CurrentFloor - floor) * ElevatorSpeedInSecondsPerFloor;
        }

        // We need to calculate all stops made by the elevator before it reaches the floor
        // And also all floors travelled. 
        var tempDestinationFloors = elevator.DestinationFloors.ToList();
        int timeToFloor =
        (
            Math.Abs(elevator.CurrentFloor - elevator.NextFloor)
            * ElevatorSpeedInSecondsPerFloor
        ) + DoorOpenTimeInSeconds;
        var previousFloor = elevator.NextFloor;
        int numStops = 0;
        if (elevator.Direction == Direction.Up && wantedDirectionUp)
        {
            numStops = tempDestinationFloors.Count(x => x > previousFloor && x < floor);
            var numFloorTravels = tempDestinationFloors.Max() - previousFloor;
            timeToFloor +=
                (numFloorTravels * ElevatorSpeedInSecondsPerFloor)
                + (numStops * DoorOpenTimeInSeconds);
        }
        if (elevator.Direction == Direction.Down && !wantedDirectionUp)
        {
            numStops = tempDestinationFloors.Count(x => x < previousFloor && x > floor);
            var numFloorTravels = previousFloor - tempDestinationFloors.Min();
            timeToFloor +=
                (numFloorTravels * ElevatorSpeedInSecondsPerFloor)
                + (numStops * DoorOpenTimeInSeconds);
        }
        if (elevator.Direction == Direction.Up && !wantedDirectionUp)
        {
            numStops += tempDestinationFloors.Count(x => x > previousFloor);
            var otherDirection = elevator.NextDirectionDestinationFloors.ToList();
            numStops += otherDirection.Count(x => x > floor);
            var numFloorTravels = tempDestinationFloors.Count == 0 ? 0 : tempDestinationFloors.Max() - previousFloor;
            numFloorTravels += tempDestinationFloors.Count == 0 ? previousFloor - floor : tempDestinationFloors.Max() - floor;
            timeToFloor +=
                (numFloorTravels * ElevatorSpeedInSecondsPerFloor)
                + (numStops * DoorOpenTimeInSeconds);
        }
        if (elevator.Direction == Direction.Down && wantedDirectionUp)
        {
            numStops += tempDestinationFloors.Count(x => x < previousFloor);
            var otherDirection = elevator.NextDirectionDestinationFloors.ToList();
            numStops += otherDirection.Count(x => x < floor);
            var numFloorTravels = tempDestinationFloors.Count == 0 ? 0 : previousFloor - tempDestinationFloors.Min();
            numFloorTravels += tempDestinationFloors.Count == 0 ? floor - previousFloor : floor - tempDestinationFloors.Min();
            timeToFloor +=
                (numFloorTravels * ElevatorSpeedInSecondsPerFloor)
                + (numStops * DoorOpenTimeInSeconds);
        }
        return timeToFloor;
    }
}
