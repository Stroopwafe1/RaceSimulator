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
			Track elburg = new Track("Circuit Elburg", new [] { SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.StartGrid, SectionTypes.StartGrid });
			Competition.Tracks.Enqueue(elburg);
			Competition.Tracks.Enqueue(new Track("A", new[] { SectionTypes.StartGrid, SectionTypes.Finish }));
			Competition.Tracks.Enqueue(new Track("B", new[] { SectionTypes.StartGrid, SectionTypes.Finish }));
			Competition.Tracks.Enqueue(new Track("C", new[] { SectionTypes.StartGrid, SectionTypes.Finish }));
		}

		public static void NextRace() {
			Track nextTrack = Competition.NextTrack();
			if(nextTrack != null) {
				CurrentRace = new Race(nextTrack, Competition.Participants);
			}
		}
	}
}