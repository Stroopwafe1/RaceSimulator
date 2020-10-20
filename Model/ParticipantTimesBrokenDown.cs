using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class ParticipantTimesBrokenDown : IParticipantData {

        public string Name { get; set; }
        public int Count { get; set; }
        public IParticipant Participant { get; set; }

        public void Add(List<IParticipantData> participantData) {
            var participant = participantData.FirstOrDefault(data => data.Participant.Name == this.Name);
            if(participant == null) {
                participantData.Add(this);
                return;
            }
            ParticipantTimesBrokenDown timesBrokenDown = (ParticipantTimesBrokenDown)participant;
            timesBrokenDown.Count += this.Count;
        }

        public string GetBestParticipant(List<IParticipantData> participantData) {
            var brokenDownData = participantData.Cast<ParticipantTimesBrokenDown>().ToList();
            int maxPoints = Int32.MaxValue;
            string bestParticipant = "";
            foreach (ParticipantTimesBrokenDown brokenDown in brokenDownData) {
                if (brokenDown.Count < maxPoints) {
                    maxPoints = brokenDown.Count;
                    bestParticipant = brokenDown.Name;
                }
            }
            return bestParticipant;
        }
    }
}
