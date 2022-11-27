# Elevator Simulator

## Summary
An elevator simulator in your console. Simulate any number of elevators and send them to floors or call from floors. Watch them go up and down and smile. 

## Installation
Clone repository, move into ControlPanel folder and run `dotnet run` in console.

## Usage
This project includes an elevator control panel that can be run in your console. This is for demonstration purposes only, the code in that project is a mess, please ignore :)

The elevator control panel aims to demonstrate the features of the ElevatorController. When you start the control panel you get to choose the number of elevators to simulate and the first floors to send the elevator to. After that you may (c)all an elevator, (a)dd destination floors to an elevator, get (s)tatus of all elevators or hit the (e)mergency break.

Updates on the elevators travels will be printed in the console. 

## Features
This Elevator Simulator aims to move each elevator to its destination floors in the most efficient order. Each elevator has its own queue of destinations which can be updated either by pressing buttons in the elevator: `Elevator.AddDestinationFloors(List<int> floors, bool up)` 
or by calling the elevator from the floor you are on: `ElevatorController.CallElevator(int fromFloor, bool up)`

When you have multiple elevators and you call for an elevator the ElevatorController will try to find the elevator closest to you going in the same direction. 

## Known limitations and bugs
* With many destinations in queue in different directions in several different elevators the ElevatorController will not allways find the most efficient elevator when calling for one.
* It does not recalculate the closest elevator after calling it. This means that if someone in the elevator adds more destinations after it was called this is not taken into account. 
* Task Parallell Library (TPL) is being used in the simulation of elevator movement. This means that we are depending on the scheduling of TPL which might not be completely realistic. In practice one should not notice this though and it's close enough for this purpose.
