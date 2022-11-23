namespace ElevatorController
{
    public class Elevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public int DestinationFloor { get; set; }
        public int NextDestinationFloor { get; set; }
        public ElevatorState State { get; set; }

        public Elevator()
        {
            State = ElevatorState.Idle;
        }
    }
}