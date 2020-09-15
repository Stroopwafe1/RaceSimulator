using System;
using Controller;
using Model;

namespace RaceSimulator {
	class Program {
		static void Main(string[] args) {
			Data.Initialise(new Competition());
			Data.NextRace();
			Console.WriteLine($"Name of current track: {Data.CurrentRace.Track.Name}");
		}
	}
}