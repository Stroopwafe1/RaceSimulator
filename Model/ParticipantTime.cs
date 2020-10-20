using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class ParticipantTime : IParticipantData {
        public string Name { get; set; }
        public TimeSpan Time { get; set; }
        public IParticipant Participant { get; set; }

        public virtual void Add(List<IParticipantData> participantData) {
            var timeData = participantData.Cast<ParticipantTime>();
            var participant = timeData.FirstOrDefault(data => data.Name == this.Name);
            if (participant == null) {
                participantData.Add(this);
                return;
            }
            participant.Time += this.Time;
        }

        public string GetBestParticipant(List<IParticipantData> participantData) {
            var timeData = participantData.Cast<ParticipantTime>();
            TimeSpan maxTime = TimeSpan.MaxValue;
            string bestParticipant = "";
            foreach (ParticipantTime time in timeData) {
                if (TimeSpan.Compare(time.Time, maxTime) < 0) {
                    maxTime = time.Time;
                    bestParticipant = time.Name;
                }
            }
            return bestParticipant;
        }
    }
}
