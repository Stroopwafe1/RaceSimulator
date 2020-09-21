using System;
using Controller;
using Model;

namespace RaceSimulator {
	class Program {
		static void Main(string[] args) {
			Data.Initialise(new Competition());
			Data.NextRace();
			Visualiser.DrawTrack(Data.CurrentRace.Track);
			Console.ReadLine();
		}
	}
}