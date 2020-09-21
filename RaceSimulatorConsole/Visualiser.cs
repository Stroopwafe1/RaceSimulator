using System;
using System.Collections.Generic;
using System.Linq;
using Model;

namespace RaceSimulator {
	public static class Visualiser {
		private static int compass = 0;
		private static List<GridSquare> GridSquares = new List<GridSquare>();

		#region graphics

		private static readonly string[] _finishHorizontal = {"════", "  ░ ", "  ░ ", "════"};
		private static readonly string[] _finishVertical = {"║  ║", "║  ║", "║≡≡║", "║  ║"};
		private static readonly string[] _startHorizontal = {"════", " ▌  ", " ▌  ", "════"};
		private static readonly string[] _startVertical = {"║  ║", "║▄▄║", "║  ║", "║  ║"};
		private static readonly string[] _cornerSW = {"═╗  ", "  \\╗", "\\  ║", "║  ║"};
		private static readonly string[] _cornerSE = {"  ╔═", "╔/  ", "║  /", "║  ║"};
		private static readonly string[] _cornerNW = {"║  ║", "/  ║", "  /╝", "═╝  "};
		private static readonly string[] _cornerNE = {"║  ║", "║  \\", "╚\\  ", "  ╚═"};
		private static readonly string[] _straightHorizontal = {"════", "    ", "    ", "════"};
		private static readonly string[] _straightVertical = {"║  ║", "║  ║", "║  ║", "║  ║"};
		private static readonly string[] _empty = {"    ", "    ", "    ", "    "};
		
		/*
		  ════			║  ║				═╗  				║  ║				  ╔═				║  ║
             ░  			║  ║				  \╗				/  ║				╔/  				║  \
             ░  			║≡≡║			\  ║				  /╝				║  /				╚\  
          ════			║  ║				║  ║				═╝  				║  ║				  ╚═
		 */

		#endregion
		
		public static void Initialise() {
			
		}

		public static void DrawTrack(Track track) {
			// foreach (Section trackSection in track.Sections) {
			// 	DrawSection(trackSection.SectionType);
			// }
			CalculateGrid(track.Sections);
			Console.WriteLine($"Lowest values in the grid: X: {GridSquare.LowestX}, Y: {GridSquare.LowestY}");
			MoveGrid(Math.Abs(GridSquare.LowestX) + 1, Math.Abs(GridSquare.LowestY));
			GridSquares = GridSquares.OrderBy(_square => _square.Y).ToList();
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

		private static GridSquare GetGridSquare(int x, int y) {
			GridSquare square = null;
			try {
				square = GridSquares.Find(_square => _square.X == x && _square.Y == y);
			} catch (Exception e) {
				square = null;
			}

			return square;
		}

		private static void DrawSection(SectionTypes type) {
			switch (type) {
				case SectionTypes.StartGrid:
					if(compass == 1 || compass == 3)
						printVariable(_startHorizontal);
					else 
						printVariable(_startVertical);
					break;
				case SectionTypes.Straight:
					if(compass == 1 || compass == 3)
						printVariable(_straightHorizontal);
					else 
						printVariable(_straightVertical);
					break;
				case SectionTypes.LeftCorner:
					if(compass == 1)
						printVariable(_cornerNW);
					else if(compass == 2)
						printVariable(_cornerNE);
					else if(compass == 3)
						printVariable(_cornerSE);
					else 
						printVariable(_cornerSW);
					compass = (compass - 1) % 4;
					break;
				case SectionTypes.RightCorner:
					if(compass == 1)
						printVariable(_cornerSW);
					else if(compass == 2)
						printVariable(_cornerNW);
					else if(compass == 3)
						printVariable(_cornerNE);
					else 
						printVariable(_cornerSE);
					compass = (compass + 1) % 4;
					break;
				case SectionTypes.Finish:
					if(compass == 1 || compass == 3)
						printVariable(_finishHorizontal);
					else 
						printVariable(_finishVertical);
					break;
			}
		}

		private static void printVariable(string[] variable) {
			foreach (string s in variable) {
				Console.WriteLine(s);
			}
		}

		private static void CalculateGrid(LinkedList<Section> sections) {
			int comp = compass;
			int x = 0, y = 0;
			foreach (Section section in sections) {
				SectionTypes type = section.SectionType;
				Console.WriteLine($"Type: {type} [X,Y]: [{x},{y}]");
				switch (type) {
					case SectionTypes.StartGrid:
						if (comp == 1 || comp == 3)
							GridSquares.Add(new GridSquare(x, y, _startHorizontal));
						else 
							GridSquares.Add(new GridSquare(x, y, _startVertical));
						break;
					case SectionTypes.Straight:
						if(comp == 1 || comp == 3)
							GridSquares.Add(new GridSquare(x, y, _straightHorizontal));
						else 
							GridSquares.Add(new GridSquare(x, y, _straightVertical));
						break;
					case SectionTypes.LeftCorner:
						if(comp == 1)
							GridSquares.Add(new GridSquare(x, y, _cornerNW));
						else if(comp == 2)
							GridSquares.Add(new GridSquare(x, y, _cornerNE));
						else if(comp == 3)
							GridSquares.Add(new GridSquare(x, y, _cornerSE));
						else 
							GridSquares.Add(new GridSquare(x, y, _cornerSW));
						comp = (comp - 1) % 4;
						break;
					case SectionTypes.RightCorner:
						if(comp == 1)
							GridSquares.Add(new GridSquare(x, y, _cornerSW));
						else if(comp == 2)
							GridSquares.Add(new GridSquare(x, y, _cornerNW));
						else if(comp == 3)
							GridSquares.Add(new GridSquare(x, y, _cornerNE));
						else 
							GridSquares.Add(new GridSquare(x, y, _cornerSE));
						comp = (comp + 1) % 4;
						break;
					case SectionTypes.Finish:
						if(comp == 1 || comp == 3)
							GridSquares.Add(new GridSquare(x, y, _finishHorizontal));
						else 
							GridSquares.Add(new GridSquare(x, y, _finishVertical));
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