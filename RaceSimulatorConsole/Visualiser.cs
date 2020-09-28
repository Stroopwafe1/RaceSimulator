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

		private static readonly string[] _finishHorizontal = { "════", "  ░ ", "  ░ ", "════" };
		private static readonly string[] _finishVertical = { "║  ║", "║  ║", "║≡≡║", "║  ║" };
		private static readonly string[] _startHorizontal = { "════", "  ▄ ", " ▀  ", "════" };
		private static readonly string[] _startVertical = { "║  ║", "║▄ ║", "║ ▄║", "║  ║" };
		private static readonly string[] _cornerSW = { "═╗  ", "  \\╗", "\\  ║", "║  ║" };
		private static readonly string[] _cornerSE = { "  ╔═", "╔/  ", "║  /", "║  ║" };
		private static readonly string[] _cornerNW = { "║  ║", "/  ║", "  /╝", "═╝  " };
		private static readonly string[] _cornerNE = { "║  ║", "║  \\", "╚\\  ", "  ╚═" };
		private static readonly string[] _straightHorizontal = { "════", "    ", "    ", "════" };
		private static readonly string[] _straightVertical = { "║  ║", "║  ║", "║  ║", "║  ║" };
		private static readonly string[] _empty = {"    ", "    ", "    ", "    "};
		
		/*
		  ════			║  ║				═╗  				║  ║				  ╔═				║  ║
             ░  			║  ║				  \╗				/  ║				╔/  				║  \
             ░  			║≡≡║			\  ║				  /╝				║  /				╚\  
          ════			║  ║				║  ║				═╝  				║  ║				  ╚═
		 */

		#endregion
		
		public static void Initialise() {
			compass = 1;
			GridSquares = new List<GridSquare>();
		}

		public static void DrawTrack(Track track) {
			// foreach (Section trackSection in track.Sections) {
			// 	DrawSection(trackSection.SectionType);
			// }
			CalculateGrid(track.Sections);
			Console.WriteLine($"Lowest values in the grid: X: {GridSquare.LowestX}, Y: {GridSquare.LowestY}");
			MoveGrid(Math.Abs(GridSquare.LowestX) + 1, Math.Abs(GridSquare.LowestY));
			GridSquares = GridSquares.OrderBy(_square => _square.Y).ToList();
			GridSquares.ForEach(_square => _square.Section = InsertParticipants(_square.Section, _square.SectionData.Left, _square.SectionData.Right));
			int maxX = GridSquares.Max(_square => _square.X);
			int maxY = GridSquares.Max(_square => _square.Y);
			for (int y = 0; y <= maxY; y++) {
				for (int internalY = 0; internalY < 4; internalY++) {
					for (int x = 0; x <= maxX; x++) {
						GridSquare square = GetGridSquare(x, y);
						Console.Write(square == null ? _empty[internalY] : square.Section[internalY]);
					}
					Console.WriteLine();
				}
			}
		}

		public static string[] InsertParticipants(string[] track, IParticipant leftParticipant, IParticipant rightParticipant) {
			char initial1 = leftParticipant?.Name[0] ?? ' ';
			char initial2 = rightParticipant?.Name[0] ?? ' ';
			string[] returnValue = new [] {
				track[0],
				track[1].Replace('▄', initial1),
				track[2].Replace('▀', initial2),
				track[3]
			};
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
			foreach (Section section in sections) {
				SectionTypes type = section.SectionType;
				SectionData data = race.GetSectionData(section);
				Console.WriteLine($"Type: {type} [X,Y]: [{x},{y}]");
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