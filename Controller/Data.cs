using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace Controller {
	public static class Data {
		public static Competition Competition { get; set; }
		public static Race CurrentRace { get; set; }

		public static void Initialise(Competition competition) {
			Competition = competition;
			AddParticipants();
			AddTracks();
		}

		static void AddParticipants() {
			Competition.Participants.Add(new Driver());
			Competition.Participants.Add(new Driver());
			Competition.Participants.Add(new Driver());
			Competition.Participants.Add(new Driver());
		}

		static void AddTracks() {
			Competition.Tracks.Enqueue(new Track("A", new[] { new Section(), new Section() }));
			Competition.Tracks.Enqueue(new Track("B", new[] { new Section(), new Section() }));
			Competition.Tracks.Enqueue(new Track("C", new[] { new Section(), new Section() }));
		}

		public static void NextRace() {
			Track nextTrack = Competition.NextTrack();
			if(nextTrack != null) {
				CurrentRace = new Race(nextTrack, Competition.Participants);
			}
		}
	}
}