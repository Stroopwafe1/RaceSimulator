﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
	public class Track {
		public string Name { get; set; }
		public LinkedList<Section> Sections { get; set; }

		public Track(string name, SectionTypes[] sections) {
			Name = name;
			Sections = ConvertSectionTypesToSections(sections);
		}

		private LinkedList<Section> ConvertSectionTypesToSections(SectionTypes[] sectionTypes) {
			LinkedList<Section> returnValue = new LinkedList<Section>();
			foreach (SectionTypes sectionType in sectionTypes) {
				Section section = new Section(sectionType);
				returnValue.AddLast(section);
			}
			
			return returnValue;
		}
	}
}
