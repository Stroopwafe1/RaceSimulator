using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace RaceSimulatorGUI {
    public class ParticipantRankingDisplay : IParticipantDisplay {

        public IParticipant Participant { get; set; }
        public int Place { get; set; }
        public string Name { get; set; }
        public Brush Brush { get; set; }
        public bool Finished { get; set; }

        public ParticipantRankingDisplay(IParticipant participant, int place, bool finished) {
            Participant = participant;
            Place = place;
            Name = participant.Name;
            switch (participant.TeamColour) {
                case TeamColours.Red:
                    Brush = new SolidColorBrush(Color.FromRgb(255, 15, 15));
                    break;
                case TeamColours.Green:
                    Brush = new SolidColorBrush(Color.FromRgb(15, 255, 15));
                    break;
                case TeamColours.Yellow:
                    Brush = new SolidColorBrush(Color.FromRgb(255, 255, 15));
                    break;
                case TeamColours.Grey:
                    Brush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                    break;
                case TeamColours.Blue:
                    Brush = new SolidColorBrush(Color.FromRgb(15, 255, 255));
                    break;
            }
            Finished = finished;
            Brush.Freeze();
        }
    }
}
