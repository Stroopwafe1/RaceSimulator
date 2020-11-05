using System;
using System.Drawing;
using NUnit.Framework;
using RaceSimulatorGUI;

namespace ControllerTest.GUI {
	[TestFixture]
	public class GUI_ImageLoader {

		[SetUp]
		public void Setup() {
			ImageLoader.Initialise();
		}

		[Test]
		public void ClearCache_CacheEmpty() {
			ImageLoader.CreateEmptyBitmap(10, 10);
			ImageLoader.ClearCache();
			Assert.IsEmpty(ImageLoader.GetCache());
		}

		[Test]
		public void GetImageFromCache() {
			Bitmap expected = new Bitmap(@"C:\Users\Lilith\Pictures\Programming Resources\Racetrack_Car_Red.png");
			Bitmap actual = ImageLoader.GetImageFromCache(@"C:\Users\Lilith\Pictures\Programming Resources\Racetrack_Car_Red.png");
			Assert.IsNotEmpty(ImageLoader.GetCache());
			Assert.AreEqual(expected.Size, actual.Size);
		}

		[Test]
		public void CreateBitmapSource_BitmapNull_ThrowsException() {
			Assert.Throws<ArgumentNullException>(() => ImageLoader.CreateBitmapSourceFromGdiBitmap(null));
		}
	}
}