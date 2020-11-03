using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaceSimulatorGUI {
    public class GridSquare {

		public int X { get; set; }
		public int Y { get; set; }
		public static int LowestX { get; set; }
		public static int LowestY { get; set; }
		public string ImagePath { get; set; }
		public int Compass { get; set; }
		public bool Flip { get; set; }

		public SectionData SectionData { get; set; }

		public GridSquare(int x, int y, string imagePath, SectionData sectionData, int compass) : this(x, y, imagePath, sectionData, compass, false){ }

		public GridSquare(int x, int y, string imagePath, SectionData sectionData, int compass, bool flip) {
			X = x;
			Y = y;
			ImagePath = imagePath;
			SectionData = sectionData;
			Compass = compass;
			Flip = flip;
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
