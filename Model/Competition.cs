using System;
using System.Collections.Generic;
using System.Text;

namespace Model {
	public class Competition {

		public List<IParticipant> Participants { get; set; }
		public Queue<Track> Tracks { get; set; }

		public Track NextTrack() {
			Track returnTrack = null;
			Tracks.TryDequeue(out returnTrack);
			return returnTrack;
		}

		public Competition() {
			Participants = new List<IParticipant>();
			Tracks = new Queue<Track>();
		}
	}
}
