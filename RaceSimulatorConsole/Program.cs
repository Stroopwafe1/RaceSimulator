using System;
using System.Threading;
using Controller;
using Model;

namespace RaceSimulator {
	class Program {
		static void Main(string[] args) {
			Data.Initialise(new Competition());
			Data.NextRace();
			Visualiser.Initialise();
			Visualiser.DrawTrack(Data.CurrentRace.Track);
			//Console.ReadLine();

			for(; ; )
				{
				Thread.Sleep(100);
			}
		}
	}
}