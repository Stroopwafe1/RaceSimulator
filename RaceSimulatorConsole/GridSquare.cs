﻿namespace RaceSimulator {
	public class GridSquare {
		public int X { get; set; }
		public int Y { get; set; }
		public static int LowestX { get; set; }
		public static int LowestY { get; set; }
		public string[] Section { get; set; }

		public GridSquare(int x, int y, string[] section) {
			X = x;
			Y = y;
			Section = section;
			SetLowestCoordinates(x, y);
		}

		private void SetLowestCoordinates(int x, int y) {
			if (x < LowestX)
				LowestX = x;
			if (y < LowestY)
				LowestY = y;
		}
	}
}