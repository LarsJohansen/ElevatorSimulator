using System.Collections.Concurrent;

namespace ElevatorController
{
    public class Elevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; } 
        public ConcurrentQueue<int> DestinationFloors { get; set; } = new ConcurrentQueue<int>();
        public ConcurrentQueue<int> NextDirectionDestinationFloors { get; set; } = new ConcurrentQueue<int>();
        public Direction Direction { get; private set; } = Direction.Stationary;
        public ElevatorState State { get; set; } = ElevatorState.Idle;
        public int NextFloor { get; set; }
        //TODO: Vurder å fjerne denne og bruke lokale variabler i stedet
        public Task? task { get; set; }
        private static CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public Elevator(int id, int currentFloor = 1)
        {
            Id = id;
            CurrentFloor = currentFloor;
        }

        public void AddDestinationFloor(int floor, bool up)
        {
            var floorList = new List<int>(){floor};
            AddDestinationFloors(floorList, up);
        }
        public void AddDestinationFloors(List<int> floors, bool up)
        {
            if (State == ElevatorState.Idle && CurrentFloor == 1)
            {
                foreach (var floor in floors.OrderBy(x => x))
                {
                    DestinationFloors.Enqueue(floor);
                }
                NextFloor = DestinationFloors.TryDequeue(out var nextFloor) ? nextFloor : 0;
                StartElevator();
            }
            else if (State == ElevatorState.Idle && CurrentFloor != 1)
            {
                if (up) 
                {
                    SortForGoingUp(floors, up);
                }
                else
                {
                    SortForGoingDown(floors, up);
                }
                StartElevator();
            }
            else
            {
                if (Direction == Direction.Up)
                {
                    SortForGoingUp(floors, up);
                }
                else if (Direction == Direction.Down)
                {
                    SortForGoingDown(floors, up);
                }
            }
        }

        private void StartElevator()
        {
            State = ElevatorState.Moving;
            SetDirection();
            task = Task.Factory.StartNew(() => MoveElevator(), _tokenSource.Token);
        }

        private void SortForGoingDown(List<int> floors, bool up)
        {
            if (!up)
            {
                List<int> mergedFloors = MergeFloors(floors);
                foreach (var floor in mergedFloors.OrderBy(x => x).Reverse())
                {
                    if (floor < CurrentFloor)
                    {
                        DestinationFloors.Enqueue(floor);
                    }
                }
                foreach (var floor in mergedFloors.OrderBy(x => x))
                {
                    if (floor > CurrentFloor)
                    {
                        DestinationFloors.Enqueue(floor);
                    }
                }
                if (State != ElevatorState.DoorOpen)
                {
                    NextFloor = DestinationFloors.TryDequeue(out var nextFloor) ? nextFloor : 0;
                }
            }
            else 
            {
                foreach (var floor in floors.OrderBy(x => x))
                {
                    NextDirectionDestinationFloors.Enqueue(floor);
                }
            }
        }


        private void SortForGoingUp(List<int> floors, bool up)
        {
            if (up)
            {
                List<int> mergedFloors = MergeFloors(floors);
                foreach (var floor in mergedFloors.OrderBy(x => x))
                {
                    if (floor > CurrentFloor)
                    {
                        DestinationFloors.Enqueue(floor);
                    }
                }
                foreach (var floor in mergedFloors.OrderBy(x => x).Reverse())
                {
                    if (floor < CurrentFloor)
                    {
                        DestinationFloors.Enqueue(floor);
                    }
                }
                if (State != ElevatorState.DoorOpen)
                {
                    NextFloor = DestinationFloors.TryDequeue(out var nextFloor) ? nextFloor : 0;
                }
            }
            else 
            {
                foreach (var floor in floors.OrderBy(x => x).Reverse())
                {
                    NextDirectionDestinationFloors.Enqueue(floor);
                }
            }
        }
        private List<int> MergeFloors(List<int> floors)
        {
            var tempFloors = new List<int>();
            while (DestinationFloors.TryDequeue(out int floor))
            {
                tempFloors.Add(floor);
            }
            tempFloors.AddRange(floors);

            if (NextFloor != 0)
            {
                tempFloors.Add(NextFloor);
            }
            return tempFloors;
        }

        private void MoveElevator()
        {
            if (NextFloor == 0)
            {
                State = ElevatorState.Idle;
                return;
            }
            while (CurrentFloor != NextFloor)
            {
                SetDirection();
                var ct = _tokenSource.Token;
                if (ct.IsCancellationRequested)
                {
                    System.Console.WriteLine($"Elevator {Id}: Emergency stop pressed before starting!");
                    ct.ThrowIfCancellationRequested();
                }
                Console.WriteLine($"Elevator {Id} is moving from {CurrentFloor} to {NextFloor}");

                for (int i = 0; i < ElevatorController.ElevatorSpeedInSecondsPerFloor; i++)
                {
                    Thread.Sleep(1000);
                    ct = _tokenSource.Token;
                    if (ct.IsCancellationRequested)
                    {
                        System.Console.WriteLine($"Elevator {Id}: Emergency stop pressed while moving!");
                        return;
                    }
                }

                if (CurrentFloor < NextFloor)
                {
                    CurrentFloor++;
                }
                else
                {
                    CurrentFloor--;
                }
                
            }

            State = ElevatorState.DoorOpen;
            Console.WriteLine($"Elevator {Id} has arrived at {CurrentFloor}. Door is open");
            
            for (int i = 0; i < ElevatorController.DoorOpenTimeInSeconds; i++)
            {
                Thread.Sleep(1000);
                var ct = _tokenSource.Token;
                if (ct.IsCancellationRequested)
                {
                    System.Console.WriteLine($"Elevator {Id}: Emergency stop pressed while door open!");
                    return;
                }
            }

            if (DestinationFloors.TryDequeue(out int nextFloor))
            {
                NextFloor = nextFloor;
                State = ElevatorState.Moving;
                MoveElevator();
            }
            else
            {
                while(NextDirectionDestinationFloors.TryDequeue(out int nextDirectionFloor))
                {
                    DestinationFloors.Enqueue(nextDirectionFloor);
                }
                if (DestinationFloors.TryPeek(out int nextDestinationFloor))
                {
                    NextFloor = nextDestinationFloor;
                    MoveElevator();
                }
                else
                {
                    State = ElevatorState.Idle;
                    Console.WriteLine($"Elevator {Id} is idle");
                }
            }
        }

        public void EmergencyStop()
        {
            _tokenSource.Cancel();
            State = ElevatorState.Failure;
            Console.WriteLine($"Elevator {Id} is has been stopped and is in {State} state!");
        }

        private void SetDirection()
        {
            if (CurrentFloor < NextFloor)
            {
                Direction = Direction.Up;
            }
            else if (CurrentFloor > NextFloor)
            {
                Direction = Direction.Down;
            }
            else if (CurrentFloor == NextFloor && State != ElevatorState.Idle)
            {
                return;
            }
            else
            {
                Direction = Direction.Stationary;
            }
        }
    }
}