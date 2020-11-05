using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace RaceSimulatorGUI {
    public interface IParticipantDisplay {
        public IParticipant Participant { get; set; }
        public int Place { get; set; }
        public string Name { get; set; }
        public Brush Brush { get; set; }

    }
}
