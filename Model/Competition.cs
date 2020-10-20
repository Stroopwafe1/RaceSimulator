using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model {
	public class Competition {

		public List<IParticipant> Participants { get; set; }
		public Queue<Track> Tracks { get; set; }
		public RaceData<ParticipantPoints> ParticipantPoints { get; set; }
		public RaceData<ParticipantTime> ParticipantTime { get; set; }
		public RaceData<ParticipantsOvertaken> ParticipantsOvertaken { get; set; }
		public RaceData<ParticipantTimesBrokenDown> ParticpantTimesBrokenDown { get; set; }

		public Track NextTrack() {
			Track returnTrack = null;
			Tracks.TryDequeue(out returnTrack);
			return returnTrack;
		}

		public Competition() {
			Participants = new List<IParticipant>();
			Tracks = new Queue<Track>();
			ParticipantPoints = new RaceData<ParticipantPoints>();
			ParticipantTime = new RaceData<ParticipantTime>();
			ParticipantsOvertaken = new RaceData<ParticipantsOvertaken>();
			ParticpantTimesBrokenDown = new RaceData<ParticipantTimesBrokenDown>();
		}

		public void AddPoints(Dictionary<int, IParticipant> finalRanking) {
			/* Points will be as follows: 
			 * First: 10 points
			 * Second: 7 points
			 * Third: 5 points
			 * 4th-6th: 3-1 points
			 */
			int points = 10;
			for(int i = 1; i <= finalRanking.Count; i++) {
				switch (i) {
					case 2:
						points -= 3;
						break;
					case 3:
					case 4:
						points -= 2;
						break;
					case 5:
					case 6:
						points -= 1;
						break;
				}
				IParticipant participant = Participants.First(p => p.Name == finalRanking[i].Name);
				participant.Points += points;
				ParticipantPoints.AddToList(new ParticipantPoints() { Name = finalRanking[i].Name, Points = points });
				Debug.WriteLine($"{points} added to Participant {finalRanking[i].Name}, they now have {participant.Points}");
            }
        }

		public void AddSectionTime(IParticipant participant, TimeSpan time, Section section) {
			ParticipantTime.AddToList(new ParticipantSectionTime() { Name = participant.Name, Section = section, Time = time });
        }

		public void AddParticipantOvertaken(IParticipant overtaker, IParticipant overtaken) {
			Debug.WriteLine($"{overtaken.Name} was overtaken by {overtaker.Name}");
			ParticipantsOvertaken.AddToList(new ParticipantsOvertaken() { OvertakenName = overtaken.Name, OvertakerName = overtaker.Name });
        }

		public void AddParticipantBrokenDown(IParticipant participant, int count) {
			ParticpantTimesBrokenDown.AddToList(new ParticipantTimesBrokenDown() { Name = participant.Name, Count = count });
        }
	}
}
