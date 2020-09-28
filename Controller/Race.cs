using Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
			if(positions.TryGetValue(section, out SectionData returnValue))
				return returnValue;
			returnValue = new SectionData();
			positions.Add(section, returnValue);
			return returnValue;
		}

		public Race(Track track, List<IParticipant> participants) {
			Track = track;
			Participants = participants;
			random = new Random(DateTime.Now.Millisecond);
			positions = new Dictionary<Section, SectionData>();
			PlaceParticipantsOnTrack();
		}

		public void RandomiseEquipment() {
			Participants.ForEach(_participant => {
				_participant.Equipment.Quality = random.Next();
				_participant.Equipment.Performance = random.Next();
				});
		}

		public void PlaceParticipantsOnTrack() {
			List<Section> startSections =  Track.Sections.Where(_section => _section.SectionType == SectionTypes.StartGrid).ToList();
			for (int i = 0; i < startSections.Count; i++) {
				SectionData sectionData = GetSectionData(startSections[i]);
				for (int j = 2 *i; j <= 2 * i + 1; j++) {
					if (j >= Participants.Count) break;
					Console.WriteLine($"Participant: {Participants[j].Name}, on {j}" );
					if (j % 2 == 0)
						sectionData.Left = Participants[j];
					else
						sectionData.Right = Participants[j];
				}
			}
		}
	}
}
