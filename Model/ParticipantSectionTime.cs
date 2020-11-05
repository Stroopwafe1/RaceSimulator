﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model {
    public class ParticipantSectionTime : ParticipantTime {

        public Section Section { get; set; }

        public override void Add(List<IParticipantData> participantData) {
            var sectionTimeData = participantData.Cast<ParticipantSectionTime>();
            var participant = sectionTimeData.FirstOrDefault(data => data.Participant.Name == Name && data.Section == Section);
            if (participant == null) {
                participantData.Add(this);
                return;
            }
            participant.Time = Time;
        }
    }
}
