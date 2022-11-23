namespace ElevatorController;
public class ElevatorController
{
    //public Dictionary<int, Elevator> Elevators { get; set; }
    private Elevator _elevator;
    public int ElevatorSpeedInSecondsPerFloor { get; set; }
    public int DoorOpenTimeInSeconds { get; set; }
    public ElevatorController(Elevator elevator)
    {
        _elevator = elevator;
        ElevatorSpeedInSecondsPerFloor = 3;
        DoorOpenTimeInSeconds = 5;
    }

    public void SetDestinationFloor(int floor)
    {
        _elevator.DestinationFloor = floor;
        _elevator.State = ElevatorState.Moving;
        Task.Factory.StartNew(() => MoveElevator());
    }

    private void MoveElevator()
    {
        while (_elevator.CurrentFloor != _elevator.DestinationFloor)
        {
            Console.WriteLine($"Elevator {_elevator.Id} is moving from {_elevator.CurrentFloor} to {_elevator.DestinationFloor}");
            if (_elevator.CurrentFloor < _elevator.DestinationFloor)
            {
                _elevator.CurrentFloor++;
            }
            else
            {
                _elevator.CurrentFloor--;
            }
            Thread.Sleep(ElevatorSpeedInSecondsPerFloor * 1000);
        }

        _elevator.State = ElevatorState.DoorOpen;
        Console.WriteLine($"Elevator {_elevator.Id} has arrived at {_elevator.CurrentFloor}. Door is open");
        Thread.Sleep(DoorOpenTimeInSeconds * 1000);

        if (_elevator.NextDestinationFloor != 0)
        {
            var nextFloor = _elevator.NextDestinationFloor;
            _elevator.NextDestinationFloor = 0;
            SetDestinationFloor(nextFloor);
        }
        else
        {
            _elevator.DestinationFloor = 0;
            _elevator.State = ElevatorState.Idle;
            Console.WriteLine($"Elevator {_elevator.Id} is idle");
        }
    }

    public void CallElevator(int fromFloor)
    {
        System.Console.WriteLine($"Elevator {_elevator.Id} is called from floor {fromFloor}");
        if (_elevator.State == ElevatorState.Idle)
        {
            SetDestinationFloor(fromFloor);
        }
        else if (_elevator.State == ElevatorState.Moving)
        {
            _elevator.NextDestinationFloor = fromFloor;
        }
    }

    public Direction GetDirection()
    {
        if (_elevator.CurrentFloor < _elevator.DestinationFloor)
        {
            return Direction.Up;
        }
        else if (_elevator.CurrentFloor > _elevator.DestinationFloor)
        {
            return Direction.Down;
        }
        else
        {
            return Direction.Idle;
        }
    }

    public int GetEstimatedTimeOfArrival(int floor)
    {
        if (_elevator.State == ElevatorState.Idle)
        {
            return Math.Abs(_elevator.CurrentFloor - floor) * ElevatorSpeedInSecondsPerFloor;
        }
        else if (_elevator.DestinationFloor == floor) 
        {
            return Math.Abs(_elevator.CurrentFloor - floor) * ElevatorSpeedInSecondsPerFloor;
        }
        else 
        {
            return Math.Abs(_elevator.CurrentFloor - _elevator.DestinationFloor) * ElevatorSpeedInSecondsPerFloor + DoorOpenTimeInSeconds
            + Math.Abs(_elevator.DestinationFloor - floor) * ElevatorSpeedInSecondsPerFloor;
        }
    }
        

    public void EmergencyStop()
    {
        _elevator.State = ElevatorState.Failure;
        _elevator.DestinationFloor = 0;
        _elevator.NextDestinationFloor = 0;
        Console.WriteLine($"Elevator {_elevator.Id} is in failure state");
    }


}
