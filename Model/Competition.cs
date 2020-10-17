using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model {
	public class Competition {

		public List<IParticipant> Participants { get; set; }
		public Queue<Track> Tracks { get; set; }
		public RaceData<ParticipantPoints> RaceData { get; set; }

		public Track NextTrack() {
			Track returnTrack = null;
			Tracks.TryDequeue(out returnTrack);
			return returnTrack;
		}

		public Competition() {
			Participants = new List<IParticipant>();
			Tracks = new Queue<Track>();
			RaceData = new RaceData<ParticipantPoints>();
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
				RaceData.AddToList(new ParticipantPoints() { Name = finalRanking[i].Name, Points = points });
				Debug.WriteLine($"{points} added to Participant {finalRanking[i].Name}, they now have {participant.Points}");
            }
        }
	}
}
