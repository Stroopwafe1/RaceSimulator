using Model;
using NUnit.Framework;
using RaceSimulatorGUI;

namespace ControllerTest.GUI {
	
	[TestFixture]
	public class GUI_GridSquare {
		
		private string s = "./test.png";
		private SectionData sectionData;
		private GridSquare square;

		[SetUp]
		public void Setup() {
			sectionData = new SectionData();
			square = new GridSquare(-4, -3, s, sectionData, 2);
		}

		[Test]
		public void LowestCoordinatesTest() {
			int expectedX = -4;
			int expectedY = -3;
			Assert.AreEqual(expectedX, GridSquare.LowestX);
			Assert.AreEqual(expectedY, GridSquare.LowestY);
		}
	}
}