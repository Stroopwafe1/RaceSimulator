using System;

namespace Model {
	public class Driver : IParticipant {
		public string Name { get; set; }
		public int Points { get; set; }
		public IEquipment Equipment { get; set; }
		public TeamColours TeamColour { get; set; }


	}
}