using System;
using System.Collections.Generic;
using System.Text;

namespace Model {
    public interface IParticipantData {

        public IParticipant Participant { get; set; }

        public void Add(List<IParticipantData> participantData);

        public string GetBestParticipant(List<IParticipantData> participantData);
    }
}
