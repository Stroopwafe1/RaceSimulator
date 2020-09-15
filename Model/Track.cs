using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
	public class Track {
		public string Name { get; set; }
		public LinkedList<Section> Sections { get; set; }

		public Track(string name, Section[] sections) {
			Name = name;
			Sections = new LinkedList<Section>(sections);
		}
	}
}
