using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class ParticipantsOvertaken : IParticipantData {

        public string OvertakerName { get; set; }
        public string OvertakenName { get; set; }
        public int Count { get; set; }
        public IParticipant Participant { get; set; }

        public ParticipantsOvertaken(string overtakerName, string overtakenName) {
            OvertakenName = overtakenName;
            OvertakerName = overtakerName;
        }

        public void Add(List<IParticipantData> participantData) {
            var overtakenData = participantData.Cast<ParticipantsOvertaken>();
            var participant = overtakenData.FirstOrDefault(data => data.OvertakerName == this.OvertakerName);
            if (participant == null) {
                this.Count++;
                participantData.Add(this);
            } else {
                participant.Count++;
            }
        }

        public string GetBestParticipant(List<IParticipantData> participantData) {
            var overtakenData = participantData.Cast<ParticipantsOvertaken>();
            int maxOvertaken = Int32.MinValue;
            string bestParticipant = "";
            foreach (ParticipantsOvertaken overtaken in overtakenData) {
                if(overtaken.Count > maxOvertaken) {
                    maxOvertaken = overtaken.Count;
                    bestParticipant = overtaken.OvertakerName;
                }
            }
            return bestParticipant;
        }
    }
}
