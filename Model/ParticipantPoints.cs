using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class ParticipantPoints : IParticipantData {

        public string Name { get; set; }
        public int Points { get; set; }
        public IParticipant Participant { get; set; }

        public void Add(List<IParticipantData> participantData) {
            var pointData = participantData.Cast<ParticipantPoints>();
            var participant = pointData.FirstOrDefault(data => data.Name == this.Name);
            if(participant == null) {
                participantData.Add(this);
                return;
            }
            participant.Points += this.Points;
        }

        public string GetBestParticipant(List<IParticipantData> participantData) {
            var pointData = participantData.Cast<ParticipantPoints>().ToList();
            int maxPoints = Int32.MinValue;
            string bestParticipant = "";
            foreach (ParticipantPoints participantPoints in pointData) {
                if(participantPoints.Points >  maxPoints) {
                    maxPoints = participantPoints.Points;
                    bestParticipant = participantPoints.Name;
                }
            }
            return bestParticipant;
        }
    }
}
