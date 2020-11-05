using NUnit.Framework;
using RaceSimulatorGUI;

namespace ControllerTest.GUI {
	[TestFixture]
	public class GUI_Visualiser {

		[SetUp]
		public void Setup() {
			Visualiser.Initialise();
		}

		[Test]
		public void Gridsquares_NotNull() {
			Assert.IsNotNull(Visualiser.GetGridSquares());
		}
	}
}