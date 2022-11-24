namespace ElevatorController
{
    public class Elevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public int DestinationFloor { get; set; }
        public int NextDestinationFloor { get; set; }
        public ElevatorState State { get; set; }
        public Task? task { get; set; }
        public CancellationTokenSource TokenSource { get; private set; }

        public Elevator(int id, int currentFloor = 1, int destinationFloor = 0)
        {
            Id = id;
            CurrentFloor = currentFloor;
            DestinationFloor = destinationFloor;
            TokenSource = new CancellationTokenSource();
            State = ElevatorState.Idle;
        }
    }
}