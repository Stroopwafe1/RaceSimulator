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
			Competition.Participants.Add(new Driver("Bob", new Car()) { TeamColour = TeamColours.Red });
			//Competition.Participants.Add(new Driver("Kelly", new Car()));
			Competition.Participants.Add(new Driver("Jerry", new Car()) { TeamColour = TeamColours.Blue});
			Competition.Participants.Add(new Driver("Sasha", new Car()) { TeamColour = TeamColours.Green });
			Competition.Participants.Add(new Driver("Rythian", new Car()) { TeamColour = TeamColours.Grey });
			Competition.Participants.Add(new Driver("Zoey", new Car()) { TeamColour = TeamColours.Yellow });
		}

		static void AddTracks() {
			Track elburg = new Track("Circuit Elburg", new [] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner });
			Track oostendorp = new Track("Oostendorp", new SectionTypes[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.StartGrid,SectionTypes.Finish, SectionTypes.Straight,
				SectionTypes.LeftCorner, SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.RightCorner,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.RightCorner,SectionTypes.Straight, SectionTypes.RightCorner,
				SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,
				SectionTypes.LeftCorner,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight, SectionTypes.LeftCorner,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight, SectionTypes.RightCorner,
				SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,
				SectionTypes.RightCorner,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight, SectionTypes.RightCorner,SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight});
			Track mkBabypark = new Track("Baby park", new[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner });
			Track mkAirshipFortress = new Track("Airship Fortress", new[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner });
			Competition.Tracks.Enqueue(elburg);
			Competition.Tracks.Enqueue(oostendorp);
			Competition.Tracks.Enqueue(mkBabypark);
			Competition.Tracks.Enqueue(mkAirshipFortress);
		}

		public static void NextRace() {
			CurrentRace?.DisposeEventHandler();
			Track nextTrack = Competition.NextTrack();
			if(nextTrack != null) {
				PutParticipantsInOrderOfFinish();
				Competition.ClearRaceData();
				CurrentRace = new Race(nextTrack, Competition.Participants);
				CurrentRace.RaceFinished += OnRaceFinished;
			} else {
				CurrentRace = null;
				if (!Console.IsOutputRedirected) Console.Clear();
				Console.WriteLine($"Race ended!");
			}
		}

		public static void PutParticipantsInOrderOfFinish() {
			if (CurrentRace == null) return;
			Competition.Participants.Clear();
			Dictionary<int, IParticipant> finalRanking = CurrentRace.GetFinalRanking();
			for(int i = 1; i <= finalRanking.Count; i++) {
				Competition.Participants.Add(finalRanking[i]);
            }
        }

		public static void OnRaceFinished(object sender, EventArgs e) {
			Competition.AddPoints(CurrentRace.GetFinalRanking());
			NextRace();
		}
	}
}