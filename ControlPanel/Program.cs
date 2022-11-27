using ElevatorController;

Console.WriteLine("============================================");
Console.WriteLine("Welcome to our amazing Elevator Simulator!");
Console.WriteLine("============================================");
Console.WriteLine();
Console.WriteLine("How many elevators do you want to simulate?");
var numberOfElevators = int.Parse(Console.ReadLine() ?? "3");
var elevators = new List<Elevator>();
for (int i = 0; i < numberOfElevators; i++)
{
    elevators.Add(new Elevator(i + 1, 1));
}
var elevatorController = new ElevatorController.ElevatorController(elevators, 5, 10);

Console.WriteLine($"All {numberOfElevators} elevators are ready to go! They are all on floor 1.");
System.Console.WriteLine("What floor(s) do you want to go to? (separate by comma)");

string floorData = Console.ReadLine() ?? string.Empty;
string[] floors = floorData.Split(',');
if (floors.Length == 0)
{
    Console.WriteLine("No floors entered. Exiting.");
    return;
}
var elevator1 = elevatorController.Elevators.First(e => e.Id == 1);
foreach (var floor in floors)
{
    if (int.TryParse(floor, out int floorNumber))
    {
        elevator1.AddDestinationFloor(floorNumber, true);
    }
    else
    {
        Console.WriteLine($"Invalid floor number: {floor}");
    }
}
System.Console.WriteLine($"Elevator {elevator1.Id} is on its way to your floor(s).");
System.Console.WriteLine(
    $"It will be at floor {elevator1.NextFloor} in {elevatorController.GetEstimatedTimeOfArrival(elevator1, elevator1.NextFloor, true)} seconds."
);

System.Console.WriteLine(
    "You can now try to call elevators from other floors, add destination floors or create an emergency."
);
System.Console.WriteLine("Have fun!");
Console.WriteLine(
    "Press (c) to call elevator, (a) to add destination floors, (e) to hit the emergency stop button, (s) to get status or (ESC) to exit,:"
);
bool exit = false;
while (!exit)
{
    var input = Console.ReadKey();
    switch (input.Key)
    {
        case ConsoleKey.C:
            System.Console.WriteLine("Select floor to call elevator from:");
            var floor = Console.ReadLine();
            System.Console.WriteLine("Select direction you want to go (up/down):");
            var direction = Console.ReadLine();
            if (direction != "up" && direction != "down")
            {
                System.Console.WriteLine("Invalid direction. Please enter up or down");
                break;
            }
            var up = direction == "up";
            if (int.TryParse(floor, out int floorNumber))
            {
                var calledElevator = elevatorController.CallElevator(floorNumber, up);
                System.Console.WriteLine(
                    $"Elevator {calledElevator.Id} is on its way. It will be there in approximately {elevatorController.GetEstimatedTimeOfArrival(calledElevator, floorNumber, up)} seconds."
                );
            }
            else
            {
                Console.WriteLine($"Invalid floor number: {floor}");
            }
            break;
        case ConsoleKey.Escape:
            exit = true;
            break;
        case ConsoleKey.A:
            System.Console.WriteLine("Select elevator to add destination floors to:");
            var elevatorId = Console.ReadLine();
            if (int.TryParse(elevatorId, out int elevatorNumber))
            {
                var elevatorToAddFloorsTo = elevators.Find(x => x.Id == elevatorNumber);
                if (elevatorToAddFloorsTo == null)
                {
                    System.Console.WriteLine($"Elevator {elevatorNumber} does not exist.");
                    break;
                }
                bool goingUp = true;
                if (elevatorToAddFloorsTo.State == ElevatorState.Idle)
                {
                    System.Console.WriteLine("Elevator is idle. Please select a direction:");
                    var dirInput = Console.ReadLine();
                    if (dirInput != "up" && dirInput != "down")
                    {
                        System.Console.WriteLine("Invalid direction. Please enter up or down");
                        break;
                    }
                    goingUp = dirInput == "up";
                }
                else
                {
                    goingUp = elevatorToAddFloorsTo.Direction == Direction.Up;
                }
                System.Console.WriteLine("Select floor(s) to add:");
                var floorDataToAdd = Console.ReadLine() ?? string.Empty;
                string[] floorsToAdd = floorDataToAdd.Split(',');
                if (floorsToAdd.Length == 0)
                {
                    Console.WriteLine("No floors entered. Exiting.");
                    return;
                }
                foreach (var f in floorsToAdd)
                {
                    if (int.TryParse(f, out int floorNumberToAdd))
                    {
                        elevatorToAddFloorsTo.AddDestinationFloor(floorNumberToAdd, goingUp);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid floor number: {f}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Invalid elevator number: {elevatorId}");
            }
            break;
        case ConsoleKey.E:
            System.Console.WriteLine("Select elevator to hit emergency stop button on:");
            var elevatorIdToStop = Console.ReadLine();
            if (int.TryParse(elevatorIdToStop, out int elevatorNumberToStop))
            {
                var elevatorToStop = elevators.Find(x => x.Id == elevatorNumberToStop);
                if (elevatorToStop == null)
                {
                    System.Console.WriteLine($"Elevator {elevatorNumberToStop} does not exist.");
                    return;
                }
                elevatorToStop.EmergencyStop();
            }
            else
            {
                Console.WriteLine($"Invalid elevator number: {elevatorIdToStop}");
            }
            break;
        case ConsoleKey.S:
            foreach (var e in elevators)
            {
                System.Console.WriteLine(
                    $"Elevator {e.Id} is on floor {e.CurrentFloor} and is {e.State}."
                );
                if (e.State == ElevatorState.Moving)
                {
                    System.Console.WriteLine(
                        $"It is currently going {e.Direction} and will be at floor {e.NextFloor} in {elevatorController.GetEstimatedTimeOfArrival(e, e.NextFloor, e.Direction == Direction.Up)} seconds."
                    );
                }
            }
            break;
        default:
            Console.WriteLine("Invalid input. Exiting.");
            return;
    }
}
