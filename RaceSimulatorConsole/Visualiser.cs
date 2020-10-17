using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model;

namespace RaceSimulator {
	public static class Visualiser {
		private static int compass;
		private static List<GridSquare> GridSquares;

		#region graphics
		//private static readonly string[] _finishHorizontal = { "____", "  I ", "  I ", "‾‾‾‾" };
		//private static readonly string[] _finishVertical = { "|  |", "|==|", "|  |", "|  |" };
		//private static readonly string[] _startHorizontal = { "____", "   ]", " ]  ", "‾‾‾‾" };
		//private static readonly string[] _startVertical = { "|^ |", "|  |", "| ^|", "|  |" };
		//private static readonly string[] _cornerSW = { "‾‾\\ ", "  \\", "\\  |", "|  |" };
		//private static readonly string[] _cornerSE = { " /‾‾", "/   ", "|  /", "|  |" };
		//private static readonly string[] _cornerNW = { "|  |", "/  |", "   /", "__/ " };
		//private static readonly string[] _cornerNE = { "|  |", "|  \\", "\\   ", " \\__" };
		//private static readonly string[] _straightHorizontal = { "____", "    ", "    ", "‾‾‾‾" };
		//private static readonly string[] _straightVertical = { "|  |", "|  |", "|  |", "|  |" };

		private static readonly string[] _finishHorizontal = { "════", " 1░ ", "2 ░ ", "════" };
		private static readonly string[] _finishVertical = { "║ 1║", "║2 ║", "║≡≡║", "║  ║" };
		private static readonly string[] _startHorizontal = { "════", " 1▄ ", "2▀  ", "════" };
		private static readonly string[] _startVertical = { "║  ║", "║▄1║", "║2▄║", "║  ║" };
		private static readonly string[] _cornerSW = { "═╗  ", " 1\\╗", "\\2 ║", "║  ║" };
		private static readonly string[] _cornerSE = { "  ╔═", "╔/2 ", "║ 1/", "║  ║" };
		private static readonly string[] _cornerNW = { "║  ║", "/1 ║", " 2/╝", "═╝  " };
		private static readonly string[] _cornerNE = { "║  ║", "║ 1\\", "╚\\2 ", "  ╚═" };
		private static readonly string[] _straightHorizontal = { "════", " 1  ", "  2 ", "════" };
		private static readonly string[] _straightVertical = { "║  ║", "║1 ║", "║ 2║", "║  ║" };
		private static readonly string[] _empty = {"    ", "    ", "    ", "    "};
		
		/*
		  ════			║ 1║				═╗  				║  ║				  ╔═				║  ║
            1░  			║2 ║				 1\╗				/1 ║				╔/2 				║ 1\
            2░  		║≡≡║			\2 ║				 2/╝				║ 1/				╚\2 
          ════			║  ║				║  ║				═╝  				║  ║				  ╚═
		 */

		#endregion
		
		public static void Initialise() {
			compass = 1;
			GridSquares = new List<GridSquare>();
			int width = 1024;
			int height = 720;
			if (width > Console.LargestWindowWidth)
				width = Console.WindowWidth;
			if (height > Console.LargestWindowHeight)
				height = Console.WindowHeight;
			Console.SetWindowSize(width, height);
			Data.CurrentRace.DriversChanged += OnDriversChanged;
			Race.RaceStarted += OnRaceStarted;
		}

		public static void OnDriversChanged(object sender, EventArgs e) {
			DriversChangedEventArgs e1 = (DriversChangedEventArgs) e;
			DrawTrack(e1.Track);
		}

		public static void OnRaceStarted(object sender, EventArgs e) {
			RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
			e1.Race.DriversChanged += OnDriversChanged;
			Console.Clear();
			DrawTrack(e1.Race.Track);
		}
		
		public static void DrawTrack(Track track) {
			Console.SetCursorPosition(0, 0);
			CalculateGrid(track.Sections);
			MoveGrid(Math.Abs(GridSquare.LowestX), Math.Abs(GridSquare.LowestY));
			GridSquares = GridSquares.OrderBy(_square => _square.Y).ToList();
			int maxX = GridSquares.Max(_square => _square.X);
			int maxY = GridSquares.Max(_square => _square.Y);
			for (int y = 0; y <= maxY; y++) {
				for (int internalY = 0; internalY < 4; internalY++) {
					for (int x = 0; x <= maxX; x++) {
						GridSquare square = GetGridSquare(x, y);
						Console.Write(square == null ? _empty[internalY] : InsertParticipants(square.Section[internalY], square.SectionData.Left, square.SectionData.Right));
					}
					Console.WriteLine();
				}
			}
		}

		public static string InsertParticipants(string track, IParticipant leftParticipant, IParticipant rightParticipant) {
			char initial1 = leftParticipant?.Name[0] ?? ' ';
			char initial2 = rightParticipant?.Name[0] ?? ' ';
			if(leftParticipant != null && leftParticipant.Equipment.IsBroken)
				initial1 = '☺';
			if (rightParticipant != null && rightParticipant.Equipment.IsBroken)
				initial2 = '☻';
			string returnValue = track.Replace('1', initial1);
			returnValue = returnValue.Replace('2', initial2);
			return returnValue;
		}


		private static GridSquare GetGridSquare(int x, int y) {
			GridSquare square = GridSquares.Find(_square => _square.X == x && _square.Y == y);
			return square;
		}

		private static void CalculateGrid(LinkedList<Section> sections) {
			Race race = Data.CurrentRace;
			int comp = compass;
			int x = 0, y = 0;
			GridSquares?.Clear();
			GridSquare.LowestX = 0;
			GridSquare.LowestY = 0;
			foreach (Section section in sections) {
				SectionTypes type = section.SectionType;
				SectionData data = race.GetSectionData(section);
				switch (type) {
					case SectionTypes.StartGrid:
						if (comp == 1 || comp == 3)
							GridSquares.Add(new GridSquare(x, y, _startHorizontal, data));
						else 
							GridSquares.Add(new GridSquare(x, y, _startVertical, data));
						break;
					case SectionTypes.Straight:
						if(comp == 1 || comp == 3)
							GridSquares.Add(new GridSquare(x, y, _straightHorizontal, data));
						else 
							GridSquares.Add(new GridSquare(x, y, _straightVertical, data));
						break;
					case SectionTypes.LeftCorner:
						if(comp == 1)
							GridSquares.Add(new GridSquare(x, y, _cornerNW, data));
						else if(comp == 2)
							GridSquares.Add(new GridSquare(x, y, _cornerNE, data));
						else if(comp == 3)
							GridSquares.Add(new GridSquare(x, y, _cornerSE, data));
						else 
							GridSquares.Add(new GridSquare(x, y, _cornerSW, data));
						comp = (comp - 1) % 4;
						if (comp < 0)
							comp = 3;
						break;
					case SectionTypes.RightCorner:
						if(comp == 1)
							GridSquares.Add(new GridSquare(x, y, _cornerSW, data));
						else if(comp == 2)
							GridSquares.Add(new GridSquare(x, y, _cornerNW, data));
						else if(comp == 3)
							GridSquares.Add(new GridSquare(x, y, _cornerNE, data));
						else 
							GridSquares.Add(new GridSquare(x, y, _cornerSE, data));
						comp = (comp + 1) % 4;
						break;
					case SectionTypes.Finish:
						if(comp == 1 || comp == 3)
							GridSquares.Add(new GridSquare(x, y, _finishHorizontal, data));
						else 
							GridSquares.Add(new GridSquare(x, y, _finishVertical, data));
						break;
				}
				if (comp == 0) {
					y--;
				} else if (comp == 1) {
					x++;
				} else if (comp == 2) {
					y++;
				} else {
					x--;
				}
			}
		}

		private static void MoveGrid(int x, int y) {
			foreach (GridSquare square in GridSquares) {
				square.X += x;
				square.Y += y;
			}
		}
	}
}