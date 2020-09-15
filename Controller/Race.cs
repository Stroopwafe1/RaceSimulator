using Model;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Controller {
	public class Race {
		public Track Track { get; set; }
		public List<IParticipant> Participants { get; set; }
		public DateTime StartTime { get; set; }
		private Random random;
		private Dictionary<Section, SectionData> positions;
		public SectionData GetSectionData(Section section) {
			SectionData returnValue = null;
			if(positions.TryGetValue(section, out returnValue))
				return returnValue;
			returnValue = new SectionData();
			positions.Add(section, returnValue);
			return returnValue;
		}

		public Race(Track track, List<IParticipant> participants) {
			Track = track;
			Participants = participants;
			random = new Random(DateTime.Now.Millisecond);
		}

		public void RandomiseEquipment() {
			Participants.ForEach(_participant => {
				_participant.Equipment.Quality = random.Next();
				_participant.Equipment.Performance = random.Next();
				});
		}
	}
}
